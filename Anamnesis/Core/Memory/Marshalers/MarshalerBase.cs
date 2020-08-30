﻿// Concept Matrix 3.
// Licensed under the MIT license.

namespace Anamnesis.Memory.Marshalers
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using Anamnesis.Memory.Exceptions;
	using Anamnesis.Memory.Offsets;

	internal abstract class MarshalerBase : IMarshaler
	{
		protected UIntPtr address;
		protected IMemoryOffset[] offsets;

		private static readonly List<MarshalerBase> ActiveMemoryInterfaces = new List<MarshalerBase>();

		public MarshalerBase(IMemoryOffset offset)
			: this(new[] { offset })
		{
		}

		public MarshalerBase(IMemoryOffset[] offsets)
		{
			this.Name = string.Empty;

			foreach (IMemoryOffset offset in offsets)
			{
				if (string.IsNullOrEmpty(offset.Name))
				{
					this.Name += "[Unknown], ";
				}
				else
				{
					this.Name += offset.Name + ", ";
				}
			}

			this.offsets = offsets;

			this.UpdateAddress();

			lock (ActiveMemoryInterfaces)
			{
				ActiveMemoryInterfaces.Add(this);
			}

			this.Active = true;
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		public string Name
		{
			get;
			private set;
		}

		public bool Active
		{
			get;
			private set;
		}

		public static void TickAllActive()
		{
			List<MarshalerBase> memories;
			lock (ActiveMemoryInterfaces)
			{
				memories = new List<MarshalerBase>(ActiveMemoryInterfaces);
			}

			foreach (MarshalerBase memory in memories)
			{
				if (!MemoryService.ProcessIsAlive)
					return;

				// Handle cases where memory was disposed while we were ticking.
				if (!memory.Active)
					continue;

				memory.Tick();
			}
		}

		public static void DisposeAll()
		{
			List<MarshalerBase> memories;
			lock (ActiveMemoryInterfaces)
			{
				memories = new List<MarshalerBase>(ActiveMemoryInterfaces);
			}

			foreach (MarshalerBase memory in memories)
			{
				if (!memory.Active)
					continue;

				memory.Dispose();
			}
		}

		public void UpdateBaseOffset(IBaseMemoryOffset newBaseOffset)
		{
			if (this.offsets[0] is IBaseMemoryOffset)
			{
				this.offsets[0] = newBaseOffset;
				this.UpdateAddress();
			}
			else
			{
				throw new Exception("First offset in memory was not a base offset");
			}
		}

		public void UpdateAddress()
		{
			this.address = MemoryService.GetAddress(this.offsets);

			if (this.address == UIntPtr.Zero)
				throw new InvalidAddressException(this.ToString());
		}

		public virtual void Dispose()
		{
			lock (ActiveMemoryInterfaces)
			{
				ActiveMemoryInterfaces.Remove(this);
			}

			this.Active = false;
		}

		public override string ToString()
		{
			string offsetString = string.Empty;
			foreach (IMemoryOffset offset in this.offsets)
			{
				offsetString += " " + offset + ",";
			}

			offsetString = offsetString.Trim(' ', ',');
			return this.Name + ": " + offsetString + " (" + this.address + ")";
		}

		protected abstract void Tick(int attempt = 0);

		protected void RaiseChanged(string name)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
