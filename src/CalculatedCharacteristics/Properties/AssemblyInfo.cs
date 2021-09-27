#region usings

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#endregion

[assembly: ComVisible(false)]
[assembly: CLSCompliant(false)]

// In order to mock internal interfaces Moq requires the following attribute because it is strong-named.
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]