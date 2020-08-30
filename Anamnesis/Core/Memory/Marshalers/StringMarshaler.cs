﻿// Concept Matrix 3.
// Licensed under the MIT license.

namespace Anamnesis.Memory.Marshalers
{
	using System;
	using System.Text;
	using Anamnesis.Memory.Offsets;

	internal class StringMarshaler : MarshalerBase<string>
	{
		private bool zeroTerminated = true;

		public StringMarshaler(params IMemoryOffset[] offsets)
			: base(offsets, 32)
		{
		}

		protected override string Read(ref byte[] data)
		{
			if (this.zeroTerminated)
			{
				return Encoding.UTF8.GetString(data).Split('\0')[0];
			}
			else
			{
				return Encoding.UTF8.GetString(data);
			}
		}

		protected override void Write(string value, ref byte[] data)
		{
			// I don't even know, but here we are.
			value = value.Replace("\0", string.Empty);
			value += "\0\0\0\0";

			Array.Copy(Encoding.UTF8.GetBytes(value), data, 32);
		}
	}
}
