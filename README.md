![[Build](https://github.com/Zeiss-PiWeb/PiWeb-Calculated-Characteristics/actions/workflows/NET_Build_And_Test.yml)](https://github.com/Zeiss-PiWeb/PiWeb-Calculated-Characteristics/actions/workflows/NET_Build_And_Test.yml/badge.svg)
[![License](https://img.shields.io/badge/License-BSD%203--Clause-blue.svg)](https://opensource.org/licenses/BSD-3-Clause)
![GitHub release (latest SemVer)](https://img.shields.io/github/v/release/zeiss-piweb/Piweb-calculated-characteristics)
[![Nuget](https://img.shields.io/nuget/v/Zeiss.PiWeb.CalculatedCharacteristics?style=flat&logo=nuget)](https://www.nuget.org/packages/Zeiss.PiWeb.CalculatedCharacteristics/)

# PiWeb-Calculated-Characteristics

| ![Zeiss IQS Logo](gfx/logo_128x128.png) | The PiWeb-Calculated-Characteristics defines the business logic how calculated characteristics are evaluated by the quality data management system [ZEISS PiWeb](http://www.zeiss.com/industrial-metrology/en_de/products/software/piweb.html). |
|-|:-|

# Overview

- [Introduction](#introduction)
- [Installation](#installation)
- [Formula Syntax](#formulasyntax)
- [How To](#howto)

<a id="markdown-introduction" name="introduction"></a>
# Introduction

An Calculated Characteristic in PiWeb is a characteristic without a deticated measured value. Furthermore it's value is calculated by a formula. This formula allows referencing the values of other characteristics. This characteristics can be regulary measured or even calculated characteristics again. Using calculated characteristics allows to provide precalculated values for reporting without the need to write system expressions over and over again.

This repository defines the core business logic of parsing formula expressions and calculating results. It is exactly the **same** logic used by PiWeb application.

<a id="markdown-installation" name="installation"></a>
# Installation

The **PiWeb Calculated Characteristics library** is available via NuGet.

Get it at [NuGet.org](https://www.nuget.org/packages/Zeiss.PiWeb.CalculatedCharacteristics/).

```cmd
PM> Install-Package Zeiss.PiWeb.CalculatedCharacteristics
```

Or compile the library by yourself. Requirements:

* Microsoft Visual Studio 2019
* Microsoft .NET Standard 2.1 or .NET5

<a id="markdown-formulasyntax" name="formulasyntax"></a>
# Formula Syntax

## Grammar

| node |   |
| --- | --- |
| expression | *expression* '+' *term* \| *expression* '-' *term* \| *term* |
| term | *term* '\*' *factor* \| *term* '/' *factor* \| *factor* |
| factor | [+-] ( *NUMBER* \| *function* \| '"' *IDENT* '"' \| '(' *expression* ')' \| '{' *path* '}' ) |
| function | *IDENT* '(' [*argumentlist*] ')' |
| argumentlist | *expression* \| *expression* '[,;]' *argumentlist* |
| path | *pathsegment* \| *pathsegment* '/' *path* \| *pathsegment* '(' *KEY* ')' |
| pathsegment | *IDENT* \| '"' *IDENT* '"' |
| NUMBER | any floating point value (with "." as decimal separator |
| IDENT | any sequence of characters |
| KEY | any integer in range [0,65535] |

## Characteristic Paths

Characteristics are always placed in curly brackets. Per default the path is provided relative to the formula owners parent.

### Escaping

Part and characteristic names with characters `( ) { } " \ /` are supported by escaping. Escaping is possible using

1. The character is leaded by the escaping character `\`
2. The name is encapsuled by `"`. Escaping within the escaped name for `"` and `\` is done be leading escape character `\`.

There are multiple ways to allow special characters in characteristics paths. The following list shows, which escaping should be used for a specific scenario.

- {abc} -> References to characteristic ***abc*** of the same parent part or characteristic
- {abc(20)} -> References to characteristic attribute with key ***20*** of charatceristic ***abc***
- {abc\(20\)} -> References to characteristic ***abc(20)***
- {"abc(20)"} -> References to characteristic ***abc(20)***
- {abc \"20\"} -> References to characteristic ***abc"20"***
- {"abc \"20\""} -> References to characteristic ***abc"20"***
- {"abc \"20\""(20)} -> References to characteristic attribute with key ***20*** of charatceristic ***abc"20"***
- {../xyz/abc} -> References to characteristic ***abc*** of part or characrteristic ***xyz***
- {../xyz\/abc} -> Reference to characteristic ***xyz/abc***
- {../"xyz/abc"} -> Reference to characteristic ***xyz/abc***

<a id="markdown-howto" name="howto"></a>
# How To

## Simple coding example

The following sample code shows an easy way of using the API. The data normally comes from the REST API and should somehow stored in organized structures and caches. As this sample should work without remote access the data is generated manually.

````csharp
namespace MySamleApp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Zeiss.PiWeb.Api.Definitions;
    using Zeiss.PiWeb.Api.Rest.Dtos.Data;
    using Zeiss.PiWeb.CalculatedCharacteristics;
    
    internal class Program
    {
        private const string Formula1 = "1 + 2";
        private const string Formula2 = "{char1} + {Char3}";

        private static void Main(string[] args)
        {
            /***** setup dummy inspection plan  *****/
            // create the inspection plan items (normally this comes from the PiWeb-Server via REST.API)
            var part = new InspectionPlanPartDto
            {
                Path = PathInformationDto.Combine(PathInformationDto.Root, PathElementDto.Part("Part")),
                Uuid = Guid.NewGuid()
            };

            var char1 = new InspectionPlanPartDto
            {
                Path = PathInformationDto.Combine(part.Path, PathElementDto.Char("Char1")),
                Uuid = Guid.NewGuid(),
                Attributes = new AttributeDto[] { new(WellKnownKeys.Characteristic.LogicalOperationString, Formula1) }
            };

            var char2 = new InspectionPlanPartDto
            {
                Path = PathInformationDto.Combine(part.Path, PathElementDto.Char("Char2")),
                Uuid = Guid.NewGuid(),
                Attributes = new AttributeDto[] { new(WellKnownKeys.Characteristic.LogicalOperationString, Formula2) }
            };

            var char3 = new InspectionPlanPartDto
            {
                Path = PathInformationDto.Combine(part.Path, PathElementDto.Char("Char3")),
                Uuid = Guid.NewGuid()
            };

            // we need to describe how inspection plan items are related 
            var structureMap = new Dictionary<PathInformationDto, (InspectionPlanDtoBase Entity, PathInformationDto[] Children)>
            {
                { part.Path, (part, new[] { char1.Path, char2.Path, char3.Path }) },
                { char1.Path, (char1, Array.Empty<PathInformationDto>()) },
                { char2.Path, (char2, Array.Empty<PathInformationDto>()) },
                { char3.Path, (char3, Array.Empty<PathInformationDto>()) }
            };

            /***** Setup some measurement data *****/
            var measurementsData = new Dictionary<PathInformationDto, double?>
            {
                { char3.Path, 5 }
            };

            // define how we get the value for an characteristic
            double? GetMeasurementValueForPath(PathInformationDto path)
            {
                return measurementsData.TryGetValue(path, out var value) ? value : null;
            }

            /***** create factory to create calculators *****/
            // define how children of an inspection plan item are provided 
            IEnumerable<PathInformationDto> ChildPathsHandler(PathInformationDto parent)
            {
                return structureMap.TryGetValue(parent, out var item) ? item.Children : Array.Empty<PathInformationDto>();
            }

            // defines how to get the attribute value for an inspection plan item from path
            object GetValueFromAttribute(PathInformationDto path, ushort key)
            {
                return structureMap.TryGetValue(path, out var item)
                    ? item.Entity.Attributes?.FirstOrDefault(a => a.Key == key)?.Value
                    : null;
            }

            // create the calculators (once created we can reuse it)
            var factory = new AttributeBasedMathInterpreterFactory(GetValueFromAttribute, ChildPathsHandler);
            var char1Calculator = factory.GetCharacteristicCalculator(char1.Path);
            var char2Calculator = factory.GetCharacteristicCalculator(char2.Path);

            // should output "Result of Char1: 3"
            Console.Write("Result of Char1: ");
            Console.WriteLine(char1Calculator.GetResult(GetMeasurementValueForPath, (path, key, _) => GetValueFromAttribute(path, key)));

            // should output "Result of Char2: 8"
            Console.Write("Result of Char2: ");
            Console.WriteLine(char2Calculator.GetResult(GetMeasurementValueForPath, (path, key, _) => GetValueFromAttribute(path, key)));
        }
    }
}
````

The type AttributeBasedMathInterpreterFactory reads the formula from the predefined characterstic attribute. If another formula source required, inherit from SimpleMathInterpreterFactory and override method GetFormula.
