# libcmdline
Simple command line argument parser library for .NET

### Example Usage
```csharp
public class Program {
    public static void Main(string[] args) {
        CommandLineArgs cmdArgs = new CommandLineArgs();
        cmdArgs.IgnoreCase = true;
        cmdArgs.PrefixRegexPatternList.Add("/{1}");
        cmdArgs.PrefixRegexPatternList.Add("-{1,2}");
        
        cmdArgs.RegisterSpecificSwitchMatchHandler("foo", (sender, e) => {
          // handle the /foo -foo or --foo switch logic here.
          // this method will only be called for the foo switch.
          // get the value given with the switch with e.Value
        });
        
        cmdArgs.ProcessCommandLineArgs(args);
  }
}
```
