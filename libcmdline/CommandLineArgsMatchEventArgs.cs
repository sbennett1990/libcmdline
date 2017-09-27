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
	/// Container for a command line switch and its value.
	/// </summary>
	public class CommandLineArgsMatchEventArgs : EventArgs {
		private string @switch;
		private string value;
		private bool isValidSwitch = true;

		/// <summary>
		/// The command line switch.
		/// </summary>
		public string Switch {
			get {
				return this.@switch;
			}
		}

		/// <summary>
		/// The value given with the command line switch.
		/// </summary>
		public string Value {
			get {
				return this.value;
			}
		}

		/// <summary>
		/// Was this switch valid?
		/// </summary>
		public bool IsValidSwitch {
			get {
				return this.isValidSwitch;
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="switch"></param>
		/// <param name="value"></param>
		public CommandLineArgsMatchEventArgs(string @switch, string value) :
			this(@switch, value, true) {
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="switch"></param>
		/// <param name="value"></param>
		/// <param name="isValidSwitch"></param>
		public CommandLineArgsMatchEventArgs(string @switch, string value, bool isValidSwitch) {
			this.@switch = @switch;
			this.value = value;
			this.isValidSwitch = isValidSwitch;
		}
	}
}
