# Static Dictionary
An experiment to see whether implementing a dictionary with a switch statement is faster than using the ```Dictionary``` provided in ```System.Collections.Generic```.

# Limitations
Implementing a dictionary like this will limit whitch types may be used as keys to types that can be used in switch statements (Types which can be used in constant expressions). Eg.
* integer literals
* floating point literals
* character literals
* string literals
* enum values

# Implementation
```C#
using System;
using System.Collections;
using System.Collections.Generic;

namespace StaticDictionary
{
	class IntStringTestDictionary : IReadOnlyDictionary<int, string>
	{
		//Keys are only initialized once
		private const string val_0 = "one";
		private const string val_1 = "two";
		private const string val_2 = "three";
		//...
		
		//KeyValues are only initialized once
		private static readonly KeyValuePair<int,string> kv_0 = new KeyValuePair<int,string>(1, val_0);
		private static readonly KeyValuePair<int,string> kv_1 = new KeyValuePair<int,string>(2, val_1);
		private static readonly KeyValuePair<int,string> kv_2 = new KeyValuePair<int,string>(3, val_2);
		//...

		public static string GetValue(int key)
		{
			switch(key)
			{
				case 1:
					return val_0;
				case 2:
					return val_1;
				case 3:
					return val_2;
				//...
				default:
					return default(string);
			}
		}

		public static bool ContainsKey(int key)
		{
			switch(key)
			{
				case 1:
				case 2:
				case 3:
				//...
					return true;
				default:
					return false;
			}
		}
		//Implement IReadOnlyDictionary using the provided functions
    }
}
```

# Test Methodology
## Test Dictionaries
Every test dictionary has integer keys [1..N]. The corresponding value for every key is a string like the name of the number. Eg. {1:"one", 2:"two", 3:"three"}.
A test dictionary of size N is generated such that N is every number in the outer product between {1, 2, 3, 4, 5, 6, 7, 8, 9} and {1, 10, 100, 1000}. 

## Tests
1. Random Access Test
	- Create an array containing 1'000'000 random valid keys 
	- Create an array containing the corresponding value for the random key of the same value.
	- Record the time it takes to confirm that the values array is correct using the test dictionary
	- Record the time it takes to confirm that the values array is correct using the corresponding dynamic dictionary.
	
2. Contains Key Test
	- Create an array containing 1'000'000 random keys 
		- Rougly 10% of the keys are not valid keys for the specific test dictionary
	- Record the time it takes to verify how many of the keys is contained within the test dictionary
	- Record the time it takes to verify how many of the keys is contained within the dynamic dictionary

## Test Execution
* Do the following for every test dictionary
	1. Copy all key value pairs from the test dictionary into a ```System.Collections.Generic.Dictionary``` or the "dynamic dictionary"
	2. Run every test and record the results.
	3. Run every test 1000 times and record the results.

