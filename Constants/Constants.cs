using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuisBot.Constants
{
    public static class Constants
    {
        public const string ABI = @"[
	{
		'constant': false,
		'inputs': [],
		'name': 'getRatingCount',
		'outputs': [
			{
				'name': '',
				'type': 'uint256'
			}
		],
		'payable': false,
		'stateMutability': 'nonpayable',
		'type': 'function'
	},
	{
		'constant': false,
		'inputs': [],
		'name': 'getBlockNumber',
		'outputs': [
			{
				'name': '',
				'type': 'uint256'
			}
		],
		'payable': false,
		'stateMutability': 'nonpayable',
		'type': 'function'
	},
	{
		'constant': false,
		'inputs': [],
		'name': 'getTotalRating',
		'outputs': [
			{
				'name': '',
				'type': 'uint256'
			}
		],
		'payable': false,
		'stateMutability': 'nonpayable',
		'type': 'function'
	},
	{
		'constant': false,
		'inputs': [
			{
				'name': 'rating',
				'type': 'uint256'
			}
		],
		'name': 'setTotalRating',
		'outputs': [],
		'payable': false,
		'stateMutability': 'nonpayable',
		'type': 'function'
	},
	{
		'constant': false,
		'inputs': [
			{
				'name': '_newgreeting',
				'type': 'string'
			}
		],
		'name': 'setGreeting',
		'outputs': [],
		'payable': false,
		'stateMutability': 'nonpayable',
		'type': 'function'
	},
	{
		'constant': false,
		'inputs': [],
		'name': 'greet',
		'outputs': [
			{
				'name': '',
				'type': 'string'
			}
		],
		'payable': false,
		'stateMutability': 'nonpayable',
		'type': 'function'
	},
	{
		'inputs': [
			{
				'name': '_greeting',
				'type': 'string'
			}
		],
		'payable': false,
		'stateMutability': 'nonpayable',
		'type': 'constructor'
	}
]";
        public static string GenerateRandom(List<string> answers)
        {
            Random r = new Random();
            int rInt = r.Next(0, answers.Count); //for ints
            return answers[rInt];
        }
        public static Dictionary<string, int> Ratings = new Dictionary<string, int>()
        {
            { "1. I'm very disappointed :(", 1 },
            { "2. I won't buy it again", 2 },
            {"3. It was so so", 3 },
            {"4. I am happy with the quality", 4 },
            {"5. Excellent, highly recommend!!", 5 }
        };
        
    }
}