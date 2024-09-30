namespace Sample;

using System;
using System.Collections.Generic;
using System.Linq;
using Zeiss.PiWeb.Api.Definitions;
using Zeiss.PiWeb.Api.Core;
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
            Path = PathInformation.Combine(PathInformation.Root, PathElement.Part("Part")),
            Uuid = Guid.NewGuid()
        };

        var char1 = new InspectionPlanPartDto
        {
            Path = PathInformation.Combine(part.Path, PathElement.Char("Char1")),
            Uuid = Guid.NewGuid(),
            Attributes = [new(WellKnownKeys.Characteristic.LogicalOperationString, Formula1)]
        };

        var char2 = new InspectionPlanPartDto
        {
            Path = PathInformation.Combine(part.Path, PathElement.Char("Char2")),
            Uuid = Guid.NewGuid(),
            Attributes = [new(WellKnownKeys.Characteristic.LogicalOperationString, Formula2)]
        };

        var char3 = new InspectionPlanPartDto
        {
            Path = PathInformation.Combine(part.Path, PathElement.Char("Char3")),
            Uuid = Guid.NewGuid()
        };

        // we need to describe how inspection plan items are related 
        var structureMap = new Dictionary<PathInformation, (InspectionPlanDtoBase Entity, PathInformation[] Children)>
        {
            { part.Path, (part, [char1.Path, char2.Path, char3.Path]) },
            { char1.Path, (char1, []) },
            { char2.Path, (char2, []) },
            { char3.Path, (char3, []) }
        };

        /***** Setup some measurement data *****/
        var measurementsData = new Dictionary<PathInformation, double?>
        {
            { char3.Path, 5 }
        };

        // define how we get the value for an characteristic
        double? GetMeasurementValueForPath(PathInformation path)
        {
            return measurementsData.TryGetValue(path, out var value) ? value : null;
        }

        /***** create factory to create calculators *****/
        // define how children of an inspection plan item are provided 
        IEnumerable<PathInformation> ChildPathsHandler(PathInformation parent)
        {
            return structureMap.TryGetValue(parent, out var item) ? item.Children : [];
        }

        // defines how to get the attribute value for an inspection plan item from path
        object? GetValueFromAttribute(PathInformation path, ushort key)
        {
            if (!structureMap.TryGetValue(path, out var item))
                return null;
                    
            return item.Entity.Attributes.FirstOrDefault(a => a.Key == key).Value;
        }

        // create the calculators (once created we can reuse it)
        var factory = new AttributeBasedMathInterpreterFactory(GetValueFromAttribute, ChildPathsHandler);
        var char1Calculator = factory.GetCharacteristicCalculator(char1.Path);
        var char2Calculator = factory.GetCharacteristicCalculator(char2.Path);

        // should output "Result of Char1: 3"
        Console.Write("Result of Char1: ");
        Console.WriteLine(char1Calculator.GetResult(
            GetMeasurementValueForPath,
            (path, key, _) => GetValueFromAttribute(path, key)));

        // should output "Result of Char2: 8"
        Console.Write("Result of Char2: ");
        Console.WriteLine(char2Calculator.GetResult(
            GetMeasurementValueForPath,
            (path, key, _) => GetValueFromAttribute(path, key)));
    }
}