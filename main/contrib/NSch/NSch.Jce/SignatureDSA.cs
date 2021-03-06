/*
Copyright (c) 2006-2010 ymnk, JCraft,Inc. All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

  1. Redistributions of source code must retain the above copyright notice,
     this list of conditions and the following disclaimer.

  2. Redistributions in binary form must reproduce the above copyright 
     notice, this list of conditions and the following disclaimer in 
     the documentation and/or other materials provided with the distribution.

  3. The names of the authors may not be used to endorse or promote products
     derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL JCRAFT,
INC. OR ANY CONTRIBUTORS TO THIS SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

This code is based on jsch (http://www.jcraft.com/jsch).
All credit should go to the authors of jsch.
*/

using Mono.Math;
using Sharpen;

namespace NSch.Jce
{
	public class SignatureDSA : NSch.SignatureDSA
	{
		internal Signature signature;

		internal KeyFactory keyFactory;

		/// <exception cref="System.Exception"></exception>
		public virtual void Init()
		{
			signature = Signature.GetInstance("SHA1withDSA");
			keyFactory = KeyFactory.GetInstance("DSA");
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void SetPubKey(byte[] y, byte[] p, byte[] q, byte[] g)
		{
			DSAPublicKeySpec dsaPubKeySpec = new DSAPublicKeySpec(new BigInteger(y), new BigInteger
				(p), new BigInteger(q), new BigInteger(g));
			PublicKey pubKey = keyFactory.GeneratePublic(dsaPubKeySpec);
			signature.InitVerify(pubKey);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void SetPrvKey(byte[] x, byte[] p, byte[] q, byte[] g)
		{
			DSAPrivateKeySpec dsaPrivKeySpec = new DSAPrivateKeySpec(new BigInteger(x), new BigInteger
				(p), new BigInteger(q), new BigInteger(g));
			PrivateKey prvKey = keyFactory.GeneratePrivate(dsaPrivKeySpec);
			signature.InitSign(prvKey);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual byte[] Sign()
		{
			return signature.Sign ();
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void Update(byte[] foo)
		{
			signature.Update(foo);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual bool Verify(byte[] sig)
		{
			int i = 0;
			int j = 0;
			byte[] tmp;
			if (sig[0] == 0 && sig[1] == 0 && sig[2] == 0)
			{
				j = ((sig[i++] << 24) & unchecked((int)(0xff000000))) | ((sig[i++] << 16) & unchecked(
					(int)(0x00ff0000))) | ((sig[i++] << 8) & unchecked((int)(0x0000ff00))) | ((sig[i
					++]) & unchecked((int)(0x000000ff)));
				i += j;
				j = ((sig[i++] << 24) & unchecked((int)(0xff000000))) | ((sig[i++] << 16) & unchecked(
					(int)(0x00ff0000))) | ((sig[i++] << 8) & unchecked((int)(0x0000ff00))) | ((sig[i
					++]) & unchecked((int)(0x000000ff)));
				tmp = new byte[j];
				System.Array.Copy(sig, i, tmp, 0, j);
				sig = tmp;
			}
			// ASN.1
			int frst = ((sig[0] & unchecked((int)(0x80))) != 0 ? 1 : 0);
			int scnd = ((sig[20] & unchecked((int)(0x80))) != 0 ? 1 : 0);
			//System.err.println("frst: "+frst+", scnd: "+scnd);
			int length = sig.Length + 6 + frst + scnd;
			tmp = new byte[length];
			tmp[0] = unchecked((byte)unchecked((int)(0x30)));
			tmp[1] = unchecked((byte)unchecked((int)(0x2c)));
			tmp[1] += (byte)frst;
			tmp[1] += (byte)scnd;
			tmp[2] = unchecked((byte)unchecked((int)(0x02)));
			tmp[3] = unchecked((byte)unchecked((int)(0x14)));
			tmp[3] += (byte)frst;
			System.Array.Copy(sig, 0, tmp, 4 + frst, 20);
			tmp[4 + tmp[3]] = unchecked((byte)unchecked((int)(0x02)));
			tmp[5 + tmp[3]] = unchecked((byte)unchecked((int)(0x14)));
			tmp[5 + tmp[3]] += (byte)scnd;
			System.Array.Copy(sig, 20, tmp, 6 + tmp[3] + scnd, 20);
			sig = tmp;
			return signature.Verify(sig);
		}
	}
}
