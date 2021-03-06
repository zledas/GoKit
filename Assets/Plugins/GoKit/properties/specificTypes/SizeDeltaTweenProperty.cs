﻿using UnityEngine;
using System.Collections;


public sealed class SizeDeltaTweenProperty : AbstractTweenProperty
{
	private RectTransform _target;
	
	private Vector2 _originalEndValue;
	private Vector2 _startValue;
	private Vector2 _endValue;
	private Vector2 _diffValue;

	public SizeDeltaTweenProperty( Vector2 endValue, bool isRelative = false ) : base( isRelative )
	{
		_originalEndValue = endValue;
	}
	
	public override bool validateTarget( object target )
	{
		return target is RectTransform;
	}
	
	public override void prepareForUse()
	{
		_target = _ownerTween.target as RectTransform;
		// This might seem to be overkill, but on the case of Transforms that
		// have been destroyed, target == null will return false, whereas
		// target.Equals(null) will return true.  Otherwise we don't really
		// get the benefits of the nanny.
		if (_target == null || _target.Equals(null))
		{
			return;
		}

		_endValue = _originalEndValue;
		
		if( _ownerTween.isFrom )
		{
			_startValue = _isRelative ? _endValue + _target.sizeDelta : _endValue;
			_endValue = _target.sizeDelta;
		}
		else
		{
			_startValue = _target.sizeDelta;
		}
		
		if( _isRelative && !_ownerTween.isFrom )
			_diffValue = _endValue;
		else
			_diffValue = _endValue - _startValue;
	}
	
	
	public override void tick( float totalElapsedTime )
	{
		var easedTime = _easeFunction( totalElapsedTime, 0, 1, _ownerTween.duration );
		var vec = GoTweenUtils.unclampedVector2Lerp( _startValue, _diffValue, easedTime );
		
		_target.sizeDelta = vec;
	}
	
	
	public void resetWithNewEndValue( Vector2 endValue )
	{
		_originalEndValue = endValue;
		prepareForUse();
	}
	
}
