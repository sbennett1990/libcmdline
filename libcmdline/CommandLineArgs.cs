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
	///			cmdArgs.RegisterOptionMatchHandler("foo", (sender, e) => {
	///				// handle the /foo -foo or --foo switch logic here.
	///				// this method will only be called for the foo switch.
	///				// get the argument given with the switch using e.Value
	///			});
	///			cmdArgs.ProcessCommandLineArgs(args);
	///		}
	/// }
	/// </code>
	/// </example>
	/// <remarks>
	/// See http://sanity-free.org/144/csharp_command_line_args_processing_class.html for more information.
	/// </remarks>
	public class CommandLineProcessor {
		public static readonly Option InvalidOptionIdentifier = new Option("INVALID");

		private ISet<string> prefixRegexPatternList;
		private IList<string> invalidArgs;
		private IDictionary<Option, string> processed;
		private IDictionary<Option, EventHandler<OptionEventArgs>> handlers;

		private bool ignoreCase;

		public event EventHandler<OptionEventArgs> SwitchMatch;

		/// <summary>
		/// Create a new command line argument processor with default command line switch
		/// prefixes.
		/// </summary>
		public CommandLineProcessor() {
			prefixRegexPatternList = new SortedSet<string>();
			invalidArgs = new List<string>();
			processed = new Dictionary<Option, string>();
			handlers = new Dictionary<Option, EventHandler<OptionEventArgs>>();
			ignoreCase = false;

			prefixRegexPatternList.Add("/{1}");
			prefixRegexPatternList.Add("-{1}");
		}

		/// <summary>
		/// The number of valid arguments given on the command line.
		/// </summary>
		public int ArgCount {
			get {
				return processed.Keys.Count;
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
		/// Collection of option prefix regex's.
		/// </summary>
		public ISet<string> PrefixRegexPatternList {
			get {
				return prefixRegexPatternList;
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="option"></param>
		/// <param name="handler"></param>
		public void RegisterOptionMatchHandler(
			string option,
			EventHandler<OptionEventArgs> handler
		) {
			RegisterOptionMatchHandler(option, false, handler);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="option"></param>
		/// <param name="requiresArgument"></param>
		/// <param name="handler"></param>
		public void RegisterOptionMatchHandler(
			string option,
			bool requiresArgument,
			EventHandler<OptionEventArgs> handler
		) {
			Option opt = new Option(option, requiresArgument);

			if (handlers.ContainsKey(opt)) {
				handlers[opt] = handler;
			}
			else {
				handlers.Add(opt, handler);
			}
		}

		/// <summary>
		/// Parse the command line arguments and attempt to execute the handlers.
		/// </summary>
		/// <param name="args">The arguments array</param>
		public void ProcessCommandLineArgs(string[] args) {
			for (int i = 0; i < args.Length; i++) {
				string argument = ignoreCase ? args[i].ToLower() : args[i];
				string prefixPattern = matchOptionPattern(argument);

				if (string.IsNullOrEmpty(prefixPattern)) {
					/* invalid argument */
					invalidArgs.Add(argument);
					continue;
				}

				string opt;
				string arg = string.Empty;
				bool argPresent = false;

				if (argument.Contains("=")) {
					// TODO: handle case where there's nothing after the '='
					/* argument style: "<prefix>Option=Argument" */
					int idx = argument.IndexOf('=');
					opt = argument.Substring(0, idx);
					arg = argument.Substring((idx + 1), (argument.Length - opt.Length - 1));
					argPresent = true;
				}
				else {
					/* argument style: "<prefix>Option Argument" */
					/* or            : "<prefix>Option" */
					opt = args[i];
					if ((i + 1) < args.Length) {
						if (!optionUpNext(args, i)) {
							// The next item should be an argument
							i++;
							arg = args[i];
							argPresent = true;
						}
					}
				}

				string scrubbedOpt = Regex.Replace(opt, prefixPattern, "", RegexOptions.Compiled);
				Option option = new Option(scrubbedOpt, argPresent);

				if (handlers.ContainsKey(option)) {
					if (argPresent) {
						onOptionMatch(new OptionEventArgs(option, arg));
						processed.Add(option, arg);
					}
					else {
						onOptionMatch(new OptionEventArgs(option, null));
						processed.Add(option, null);
					}
				}
				else {
					/* invalid argument: no handler registered */
					onOptionMatch(new OptionEventArgs(InvalidOptionIdentifier, option.Opt, false));
					invalidArgs.Add(option.Opt);
				}
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="option"></param>
		/// <returns></returns>
		public bool HasHandler(Option option) {
			string scrubbed = string.Empty;
			foreach (string prefix in prefixRegexPatternList) {
				if (Regex.IsMatch(option.Opt, prefix, RegexOptions.Compiled)) {
					scrubbed = Regex.Replace(option.Opt, prefix, "", RegexOptions.Compiled);
					break;
				}
			}

			if (ignoreCase) {
				foreach (Option key in processed.Keys) {
					if (key.Opt.ToLower() == option.Opt.ToLower()) {
						return true;
					}
				}
			}
			else {
				return handlers.ContainsKey(option);
			}

			return false;
		}

		/// <summary>
		/// Invoke the registered handler for the provided option and argument
		/// (in the form of a OptionEventArgs object).
		/// </summary>
		/// <param name="e"></param>
		protected virtual void onOptionMatch(OptionEventArgs e) {
			if (handlers.ContainsKey(e.Option) && handlers[e.Option] != null) {
				handlers[e.Option](this, e);
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
				string prefixPattern = $"^{prefix}";

				if (Regex.IsMatch(option, prefixPattern, RegexOptions.Compiled)) {
					return prefixPattern;
				}
			}

			return string.Empty;
		}

		/// <summary>
		/// Look ahead in the args array to see if the next element is an
		/// option. Returns true if the next element is a valid option,
		/// false otherwise or if there is no next element.
		/// </summary>
		/// <param name="args"></param>
		/// <param name="i"></param>
		private bool optionUpNext(string[] args, int i) {
			if ((i + 1) >= args.Length) {
				return false;
			}

			string nextArg = args[i + 1];
			string prefixPattern = matchOptionPattern(nextArg);

			if (string.IsNullOrEmpty(prefixPattern)) {
				return false;
			}
			else {
				return true;
			}
		}
	}
}
