﻿using UnityEngine;
using System.Collections;


public sealed class PivotTweenProperty : AbstractTweenProperty
{
	private RectTransform _target;
	
	private Vector2 _originalEndValue;
	private Vector2 _startValue;
	private Vector2 _endValue;
	private Vector2 _diffValue;

	public PivotTweenProperty( Vector2 endValue, bool isRelative = false ) : base( isRelative )
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
		if (_target == null)
		{
			return;
		}

		_endValue = _originalEndValue;
		
		if( _ownerTween.isFrom )
		{
			_startValue = _isRelative ? _endValue + _target.pivot : _endValue;
			_endValue = _target.pivot;
		}
		else
		{
			_startValue = _target.pivot;
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
		
		_target.pivot = vec;
	}
	
	
	public void resetWithNewEndValue( Vector2 endValue )
	{
		_originalEndValue = endValue;
		prepareForUse();
	}
	
}
