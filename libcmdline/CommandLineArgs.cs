/*
 * Copyright (c) 2017 Scott Bennett <scottb@fastmail.com>
 *
 * Permission to use, copy, modify, and distribute this software for any
 * purpose with or without fee is hereby granted, provided that the above
 * copyright notice and this permission notice appear in all copies.
 *
 * THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
 * WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
 * ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
 * WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
 * ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
 * OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace libcmdline
{
	/// <summary>
	/// Command line arguments processor. 
	/// </summary>
	/// <example>
	/// <code>
	/// static class Program {
    ///		static void Main(string[] args) {
    ///			CommandLineArgs cmdArgs = new CommandLineArgs();
    ///			cmdArgs.IgnoreCase = true;
    ///			cmdArgs.PrefixRegexPatternList.Add("/{1}");
    ///			cmdArgs.PrefixRegexPatternList.Add("-{1,2}");
    ///			cmdArgs.RegisterSpecificSwitchMatchHandler("foo", (sender, e) => {
    ///				// handle the /foo -foo or --foo switch logic here.
    ///				// this method will only be called for the foo switch.
	///				// get the value given with the switch with e.Value
    ///			});
    ///			cmdArgs.ProcessCommandLineArgs(args);
    ///		}
	/// }
	/// </code>
	/// </example>
	/// <remarks>
	/// See http://sanity-free.org/144/csharp_command_line_args_processing_class.html for more information.
	/// </remarks>
	public class CommandLineArgs
	{
		public const string InvalidSwitchIdentifier = "INVALID";

		bool ignoreCase = false;

		IList<string> prefixRegexPatternList = new List<string>();
		List<string> invalidArgs = new List<string>();
		Dictionary<string, string> arguments = new Dictionary<string, string>();
		Dictionary<string, EventHandler<CommandLineArgsMatchEventArgs>> handlers = 
			new Dictionary<string, EventHandler<CommandLineArgsMatchEventArgs>>();
		
		public event EventHandler<CommandLineArgsMatchEventArgs> SwitchMatch;

		/// <summary>
		/// Get the number of arguments given at the command line. 
		/// </summary>
		public int ArgCount {
			get {
				return arguments.Keys.Count;
			}
		}

		public IList<string> PrefixRegexPatternList {
			get {
				return prefixRegexPatternList;
			}
		}

		/// <summary>
		/// <para>
		/// Ignore the case of the command line switches. 
		/// </para>
		/// <para>
		/// Default = true 
		/// </para>
		/// </summary>
		public bool IgnoreCase {
			get {
				return this.ignoreCase;
			}
			set {
				this.ignoreCase = value;
			}
		}

		/// <summary>
		/// Get an array of all the invalid arguments given. 
		/// </summary>
		public string[] InvalidArgs {
			get {
				return invalidArgs.ToArray();
			}
		}

		public string this[string key] {
			get {
				if (containsSwitch(key)) {
					return arguments[key];
				}
				else {
					return null;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="switchName"></param>
		/// <param name="handler"></param>
		public void registerSpecificSwitchMatchHandler(
			string switchName, 
			EventHandler<CommandLineArgsMatchEventArgs> handler
		) {
			if (handlers.ContainsKey(switchName)) {
				handlers[switchName] = handler;
			}
			else {
				handlers.Add(switchName, handler);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="switchName"></param>
		/// <returns></returns>
		public bool containsSwitch(string switchName) {
			foreach (string pattern in prefixRegexPatternList) {
				if (Regex.IsMatch(switchName, pattern, RegexOptions.Compiled)) {
					switchName = Regex.Replace(switchName, pattern, "", RegexOptions.Compiled);
				}
			}

			if (ignoreCase) {
				foreach (string key in arguments.Keys) {
					if (key.ToLower() == switchName.ToLower()) {
						return true;
					}
				}
			}
			else {
				return arguments.ContainsKey(switchName);
			}

			return false;
		}

		/// <summary>
		/// Take the command line arguments and attempt to execute the handlers. 
		/// </summary>
		/// <param name="args">The arguments array</param>
		public void processCommandLineArgs(string[] args) {
			for (int i = 0; i < args.Length; i++) {
				string cmdLineValue = ignoreCase ? args[i].ToLower() : args[i];

				foreach (string prefix in prefixRegexPatternList) {
					string switchPattern = string.Format("^{0}", prefix);

					if (Regex.IsMatch(cmdLineValue, switchPattern, RegexOptions.Compiled)) {
						cmdLineValue = Regex.Replace(cmdLineValue, switchPattern, "", RegexOptions.Compiled);

						// switch style: "<prefix>Param=Value"
						if (cmdLineValue.Contains("=")) {
							int idx = cmdLineValue.IndexOf('=');
							string n = cmdLineValue.Substring(0, idx);
							string v = cmdLineValue.Substring(idx + 1, cmdLineValue.Length - n.Length - 1);
							onSwitchMatch(new CommandLineArgsMatchEventArgs(n, v));
							arguments.Add(n, v);
						}
						// switch style: "<prefix>Param Value"
						else {
							if ((i + 1) < args.Length) {
								string @switch = cmdLineValue;
								string val = args[i + 1];
								onSwitchMatch(new CommandLineArgsMatchEventArgs(@switch, val));
								arguments.Add(cmdLineValue, val);

								i++;
							}
							else {
								onSwitchMatch(new CommandLineArgsMatchEventArgs(cmdLineValue, null));
								arguments.Add(cmdLineValue, null);
							}
						}
					}
					// invalid arg ...
					//else {
					//	onSwitchMatch(new CommandLineArgsMatchEventArgs(InvalidSwitchIdentifier, cmdLineValue, false));
					//	invalidArgs.Add(cmdLineValue);
					//}
				}
			}
		}

		/// <summary>
		/// Invoke the registered handler for the provided switch and value 
		/// (in the form of a CommandLineArgsMatchEventArgs object). 
		/// </summary>
		/// <param name="e"></param>
		protected virtual void onSwitchMatch(CommandLineArgsMatchEventArgs e) {
			if (handlers.ContainsKey(e.Switch) && handlers[e.Switch] != null) {
				handlers[e.Switch](this, e);
			}
			else if (SwitchMatch != null) {
				SwitchMatch(this, e);
			}
		}
	}
}
