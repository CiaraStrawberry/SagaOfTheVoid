using System;
using System.IO;
using UnityEngine;

namespace DFNetwork.Helpers {

	public class Serializer {
	
		public int index = 0;
		
		MemoryStream ms;
		
		byte[] _bytes;
		
		//bool serialize = true;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ls_BitSerializer"/> class.
		/// </summary>
		public Serializer() {
			Reset();
		}
		
		/// <summary>
		/// Reset the memory stream
		/// </summary>
		public void Reset() {
			index = 0;
			ms = new MemoryStream();
		}
		
		/// <summary>
		/// Sets the instance to write
		/// </summary>
		public void Serialize() {
			Reset();
		}
		
		/// <summary>
		/// Return the array of bits
		/// </summary>
		/// <returns>
		/// The array.
		/// </returns>
		public byte[] ToArray() {
			return ms.ToArray();
		}
		
		/// <summary>
		/// Gets the length of the memory stream.
		/// </summary>
		/// <value>
		/// The length of the memory stream.
		/// </value>
		public int GetByteLength {
			get {
				return index;
			}
		}
		
		#region Add Bytes
		
		/// <summary>
		/// Adds a bool to the memory stream
		/// </summary>
		/// <param name='val'>
		/// Bool Value.
		/// </param>
		public void AddBits(bool val) {
			
			byte[] bytes = new byte[sizeof(bool)];
			
			bytes = BitConverter.GetBytes(val);
			
			foreach(byte b in bytes) {
				ms.WriteByte(b);
			}
			
			index += sizeof(bool);
		}
		
		/// <summary>
		/// Adds an int to the memory stream
		/// </summary>
		/// <param name='val'>
		/// Int Value
		/// </param>
		public void AddBits(int val) {
	
			byte[] bytes = new byte[sizeof(int)];
			
			bytes = BitConverter.GetBytes(val);
			
			foreach(byte b in bytes) {
				ms.WriteByte(b);
			}
			
			index += sizeof(int);
		}
		
		/// <summary>
		/// Adds an array of ints to the memory stream
		/// </summary>
		/// <param name='val'>
		/// Value.
		/// </param>
		public void AddBits(int[] val) {
			AddBits(val.Length);
			
			foreach (int v in val) {
				// add the int
				AddBits(v);
			}
		
		}
		
		/// <summary>
		/// Adds a short to the memory stream
		/// </summary>
		/// <param name='val'>
		/// Short Value
		/// </param>
		public void AddBits(short val) {
			
			byte[] bytes = new byte[sizeof(short)];
			
			bytes = BitConverter.GetBytes(val);
			
			foreach(byte b in bytes) {
				ms.WriteByte(b);
			}
			
			index += sizeof(short);
		}
		
		/// <summary>
		/// Adds the bits.
		/// </summary>
		/// <param name='val'>
		/// Value.
		/// </param>
		public void AddBits(short[] val) {

			AddBits(val.Length);
			
			foreach (short v in val) {
				// add the int
				AddBits(v);
			}
		
		}
		
		/// <summary>
		/// Adds a float to the memory stream
		/// </summary>
		/// <param name='val'>
		/// Float Value
		/// </param>
		public void AddBits(float val) {
			
			// Need to convert float to fixed point.. to ensure determinism
			/*
			byte[] bytes = new byte[sizeof(float)];
			
			bytes = BitConverter.GetBytes(val);
			
			foreach(byte b in bytes) {
				ms.WriteByte(b);
			}
			
			index += sizeof(float);
			*/
			
			// new version converts float to int
			AddBits(toFixed(val));
		}
		
		/// <summary>
		/// Adds a float array to the bits.
		/// </summary>
		/// <param name='val'>
		/// Value.
		/// </param>
		public void AddBits(float[] val) {
			
			AddBits(val.Length);
			
			foreach (float v in val) {
				// add the int
				AddBits(v);
			}
			
		}
		
		/// <summary>
		/// Adds a double to the memory stream
		/// </summary>
		/// <param name='val'>
		/// Double Value
		/// </param>
		public void AddBits(double val) {
			
			byte[] bytes = new byte[sizeof(double)];
			
			bytes = BitConverter.GetBytes(val);
			
			foreach(byte b in bytes) {
				ms.WriteByte(b);
			}
	
			index += sizeof(double);
		}
		
		/// <summary>
		/// Adds a char to the memory stream
		/// </summary>
		/// <param name='val'>
		/// Char Value
		/// </param>
		public void AddBits(char val) {
			
			byte[] bytes = new byte[sizeof(char)];
			
			bytes = BitConverter.GetBytes(val);
			
			foreach(byte b in bytes) {
				ms.WriteByte(b);
			}
	
			index += sizeof(char);
		}
		
		/// <summary>
		/// Adds a string to the memory stream (int + char list)
		/// </summary>
		/// <param name='val'>
		/// String Value.
		/// </param>
		public void AddBits(string val) {
			
			// add the length
			AddBits(val.Length);
			
			// Iterate over every char
			foreach(char c in val) {
				AddBits(c);
			}
			
		}
	
		/// <summary>
		/// Adds a string to the memory stream (int + char list)
		/// </summary>
		/// <param name='val'>
		/// String Value.
		/// </param>
		public void AddBits(string[] val) {
			
			AddBits(val.Length);
			
			foreach (string v in val) {
				// add the length
				AddBits(v.Length);
			
				// Iterate over every char
				foreach(char c in v) {
					AddBits(c);
				}
			}
			
		}
		
		/// <summary>
		/// Adds the vector3 to the memory stream (x + y + z)
		/// </summary>
		/// <param name='val'>
		/// Vector3 Value.
		/// </param>
		public void AddBits(Vector3 val) {
		
			AddBits(val.x);
			AddBits(val.y);
			AddBits(val.z);
			
			//index += sizeof(float) * 3;
		}
	
		/// <summary>
		/// Adds the vector3 to the memory stream (x + y + z)
		/// </summary>
		/// <param name='val'>
		/// Vector3 Value.
		/// </param>
		public void AddBits(Vector3[] val) {
			
			AddBits(val.Length);
			
			foreach (Vector3 v in val) {
				AddBits(v.x);
				AddBits(v.y);
				AddBits(v.z);
			}
			
			//index += sizeof(int);// + ( val.Length * (sizeof(float) * 3));
		}
		
		/// <summary>
		/// Adds the Quaternion to the memory stream (x + y + z)
		/// </summary>
		/// <param name='val'>
		/// Quaternion Value.
		/// </param>
		public void AddBits(Quaternion val) {
		
			AddBits(val.x);
			AddBits(val.y);
			AddBits(val.z);
			AddBits(val.w);
			
			//index += sizeof(float) * 4;
		}
		
		/// <summary>
		/// Adds the Quaternion to the memory stream (x + y + z + w)
		/// </summary>
		/// <param name='val'>
		/// Quaternion Value.
		/// </param>
		public void AddBits(Quaternion[] val) {
			
			AddBits(val.Length);
			
			foreach (Quaternion v in val) {
				AddBits(v.x);
				AddBits(v.y);
				AddBits(v.z);
				AddBits(v.w);
			}
			
			
			//index += sizeof(int); //+ ( val.Length * (sizeof(float) * 4));
		}
		
		#endregion
		
		
		#region Private Functions
		
		/**
		* Convert a float to  16.16 fixed-point representation
		* @param val The value to convert
		* @return The resulting fixed-point representation
		*/
		public static int toFixed(float val) {
			return (int)(val * 65536F);
		}	
		
		/**
		* Convert a 16.16 fixed-point value to floating point
		* @param val The fixed-point value
		* @return The equivalent floating-point value.
		*/
		public static float toFloat(int val) {
			return ((float)val)/65536.0f;
		}	
		
		#endregion
		
	}
}