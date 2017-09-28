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
	/// Represents a command line option.
	/// </summary>
	public class Option : IEquatable<Option> {
		private string @opt;
		private bool requires_argument;

		/// <summary>
		/// Create a new Option object and specify whether the option requires
		/// an argument. Default is no argument required.
		/// </summary>
		public Option(string @option, bool requiresArgument = false) {
			this.@opt = @option;
			this.requires_argument = requiresArgument;
		}

		/// <summary>
		/// The option string.
		/// </summary>
		public string Opt {
			get {
				return this.@opt;
			}
		}

		/// <summary>
		/// Does this option require an argument?
		/// </summary>
		public bool RequiresArgument {
			get {
				return this.requires_argument;
			}
		}

		/// <summary>
		/// Determines whether the specified option is equal to this Option.
		/// </summary>
		public bool Equals(Option other) {
			if (other == null) {
				return false;
			}

			if (this.RequiresArgument != other.RequiresArgument) {
				return false;
			}

			return string.Equals(this.Opt, other.Opt);
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current
		/// object.
		/// </summary>
		public override bool Equals(Object obj) {
			if (obj == null) {
				return false;
			}

			Option optionObj = obj as Option;
			if (optionObj == null) {
				return false;
			}
			else {
				return Equals(optionObj);
			}
		}

		/// <summary>
		/// Get the hash code of this Option.
		/// </summary>
		public override int GetHashCode() {
			return Tuple.Create(this.Opt, this.RequiresArgument).GetHashCode();
		}

		/// <summary>
		/// Get the string representation of this Option.
		/// </summary>
		public override string ToString() {
			return this.Opt.ToString();
		}
	}
}
