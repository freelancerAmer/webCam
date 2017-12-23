using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pfz.Collections;

namespace SecureChat.Client
{
	public static class SoundEnumerator
	{
		static SoundEnumerator()
		{
			ActualValueEnumerator = new ActualValueEnumerator<byte[]>();
			Distributor = new EnumeratorDistributor<byte[]>(ActualValueEnumerator);
		}
		
		public static readonly EnumeratorDistributor<byte[]> Distributor;
		public static readonly ActualValueEnumerator<byte[]> ActualValueEnumerator;

		public static void Stop()
		{
			Distributor.Dispose();
			ActualValueEnumerator.Dispose();
		}
	}
}
