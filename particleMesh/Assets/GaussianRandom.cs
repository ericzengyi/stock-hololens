﻿using UnityEngine;
using System.Collections;

public sealed class GaussianRandom
{
	private bool _hasDeviate;
	private double _storedDeviate;
	private readonly System.Random _random;
	
	public GaussianRandom()
	{
		_random = new System.Random();
	}
	
	/// <summary>
	/// Obtains normally (Gaussian) distributed random numbers, using the Box-Muller
	/// transformation.  This transformation takes two uniformly distributed deviates
	/// within the unit circle, and transforms them into two independently
	/// distributed normal deviates.
	/// </summary>
	/// <param name="mu">The mean of the distribution.  Default is zero.</param>
	/// <param name="sigma">The standard deviation of the distribution.  Default is one.</param>
	/// <returns></returns>
	public double NextGaussian()
	{
		double mu = 0;
		double sigma = 1;

		if (_hasDeviate)
		{
			_hasDeviate = false;
			return _storedDeviate*sigma + mu;
		}
		
		double v1, v2, rSquared;
		do
		{
			// two random values between -1.0 and 1.0
			v1 = 2*_random.NextDouble() - 1;
			v2 = 2*_random.NextDouble() - 1;
			rSquared = v1*v1 + v2*v2;
			// ensure within the unit circle
		} while (rSquared >= 1 || rSquared == 0);
		
		// calculate polar tranformation for each deviate
		var polar = System.Math.Sqrt(-2*System.Math.Log(rSquared)/rSquared);
		// store first deviate
		_storedDeviate = v2*polar;
		_hasDeviate = true;
		// return second deviate
		return v1*polar*sigma + mu;
	}
}