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

namespace libcmdline {
	/// <summary>
	/// Container for a command line option and its argument.
	/// </summary>
	public class OptionEventArgs : EventArgs {
		private Option option;
		private string argument;
		private bool isValidOption = true;

		/// <summary>
		/// The command line option.
		/// </summary>
		public Option Option {
			get {
				return this.option;
			}
		}

		/// <summary>
		/// The argument given with the command line option.
		/// </summary>
		public string Argument {
			get {
				return this.argument;
			}
		}

		/// <summary>
		/// Indicates whether this is a valid option.
		/// </summary>
		public bool IsValidOption {
			get {
				return this.isValidOption;
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="option"></param>
		/// <param name="argument"></param>
		public OptionEventArgs(Option option, string argument) :
			this(option, argument, true) {
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="option"></param>
		/// <param name="argument"></param>
		/// <param name="isValidOption"></param>
		public OptionEventArgs(Option option, string argument, bool isValidOption) {
			this.option = option;
			this.argument = argument;
			this.isValidOption = isValidOption;
		}
	}
}
