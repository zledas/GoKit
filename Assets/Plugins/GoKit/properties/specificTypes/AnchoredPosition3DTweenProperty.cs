﻿using UnityEngine;
using System.Collections;


public sealed class AnchoredPosition3DTweenProperty : AbstractTweenProperty
{
	private RectTransform _target;
	
	private Vector3 _originalEndValue;
	private Vector3 _startValue;
	private Vector3 _endValue;
	private Vector3 _diffValue;

	public AnchoredPosition3DTweenProperty( Vector3 endValue, bool isRelative = false ) : base( isRelative )
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
			_startValue = _isRelative ? _endValue + _target.anchoredPosition3D : _endValue;
			_endValue = _target.anchoredPosition3D;
		}
		else
		{
			_startValue = _target.anchoredPosition3D;
		}
		
		if( _isRelative && !_ownerTween.isFrom )
			_diffValue = _endValue;
		else
			_diffValue = _endValue - _startValue;
	}
	
	
	public override void tick( float totalElapsedTime )
	{
		var easedTime = _easeFunction( totalElapsedTime, 0, 1, _ownerTween.duration );
		var vec = GoTweenUtils.unclampedVector3Lerp( _startValue, _diffValue, easedTime );
		
		_target.anchoredPosition3D = vec;
	}
	
	
	public void resetWithNewEndValue( Vector3 endValue )
	{
		_originalEndValue = endValue;
		prepareForUse();
	}
	
}
