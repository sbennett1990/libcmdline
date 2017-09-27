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

namespace libcmdline {
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
	public class CommandLineArgs {
		public const string InvalidSwitchIdentifier = "INVALID";

		private IList<string> prefixRegexPatternList;
		private IList<string> invalidArgs;
		private IDictionary<string, string> arguments;
		private IDictionary<string, EventHandler<CommandLineArgsMatchEventArgs>> handlers;

		private bool ignoreCase;

		public event EventHandler<CommandLineArgsMatchEventArgs> SwitchMatch;

		/// <summary>
		/// Create a new command line argument processor with default command line switch
		/// prefixes.
		/// </summary>
		public CommandLineArgs() {
			prefixRegexPatternList = new List<string>();
			invalidArgs = new List<string>();
			arguments = new Dictionary<string, string>();
			handlers = new Dictionary<string, EventHandler<CommandLineArgsMatchEventArgs>>();
			ignoreCase = false;

			prefixRegexPatternList.Add("/{1}");
			prefixRegexPatternList.Add("-{1}");
		}

		/// <summary>
		/// The number of arguments given on the command line.
		/// </summary>
		public int ArgCount {
			get {
				return arguments.Keys.Count;
			}
		}

		/// <summary>
		/// Ignore the case of the command line switches. Default is false.
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
		/// List of all the invalid arguments given.
		/// </summary>
		public IList<string> InvalidArgs {
			get {
				return invalidArgs;
			}
		}

		/// <summary>
		///
		/// </summary>
		public IList<string> PrefixRegexPatternList {
			get {
				return prefixRegexPatternList;
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="switchName"></param>
		/// <param name="handler"></param>
		public void RegisterSpecificSwitchMatchHandler(
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
		/// Take the command line arguments and attempt to execute the handlers.
		/// </summary>
		/// <param name="args">The arguments array</param>
		public void ProcessCommandLineArgs(string[] args) {
			for (int i = 0; i < args.Length; i++) {
				string option = ignoreCase ? args[i].ToLower() : args[i];
				string optionPattern = matchOptionPattern(option);

				if (string.IsNullOrEmpty(optionPattern)) {
					continue;
				}

				string arg = Regex.Replace(option, optionPattern, "", RegexOptions.Compiled);

				if (!handlers.ContainsKey(arg)) {
					/* invalid argument */
					onSwitchMatch(new CommandLineArgsMatchEventArgs(InvalidSwitchIdentifier, arg, false));
					invalidArgs.Add(arg);
					continue;
				}

				if (arg.Contains("=")) {
					/* switch style: "<prefix>Param=Value" */
					int idx = arg.IndexOf('=');
					string n = arg.Substring(0, idx);
					string val = arg.Substring(idx + 1, arg.Length - n.Length - 1);
					onSwitchMatch(new CommandLineArgsMatchEventArgs(n, val));
					arguments.Add(n, val);
				}
				else {
					/* switch style: "<prefix>Param Value" */
					if ((i + 1) < args.Length) {
						string @switch = arg;
						string val = args[i + 1];
						onSwitchMatch(new CommandLineArgsMatchEventArgs(@switch, val));
						arguments.Add(arg, val);

						i++;
					}
					else {
						onSwitchMatch(new CommandLineArgsMatchEventArgs(arg, null));
						arguments.Add(arg, null);
					}
				}
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="switchName"></param>
		/// <returns></returns>
		public bool HasHandler(string switchName) {
			string scrubbed = string.Empty;
			foreach (string prefix in prefixRegexPatternList) {
				if (Regex.IsMatch(switchName, prefix, RegexOptions.Compiled)) {
					scrubbed = Regex.Replace(switchName, prefix, "", RegexOptions.Compiled);
					break;
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
				return handlers.ContainsKey(switchName);
			}

			return false;
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

		/// <summary>
		/// Match the given option flag (which should be taken directly from the
		/// command line) against valid switch prefixes. Returns the pattern
		/// used by the given option flag, or the empty string if it does not match.
		/// </summary>
		/// <param name="option"></param>
		private string matchOptionPattern(string option) {
			foreach (string prefix in prefixRegexPatternList) {
				string optionPattern = $"^{prefix}";

				if (Regex.IsMatch(option, optionPattern, RegexOptions.Compiled)) {
					return optionPattern;
				}
			}

			return string.Empty;
		}
	}
}
