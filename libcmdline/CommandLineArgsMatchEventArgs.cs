/*
 * Copyright (c) 2015 Scott Bennett
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * * Redistributions of source code must retain the above copyright notice, this
 *   list of conditions and the following disclaimer.
 * 
 * * Redistributions in binary form must reproduce the above copyright notice,
 *   this list of conditions and the following disclaimer in the documentation
 *   and/or other materials provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
 * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;

namespace libcmdline
{
	/// <summary>
	/// Container for a command line switch and its value. 
	/// </summary>
	public class CommandLineArgsMatchEventArgs : EventArgs
	{
		string @switch;
		string value;
		bool isValidSwitch = true;

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
