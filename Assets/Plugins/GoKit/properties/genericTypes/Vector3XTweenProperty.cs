using UnityEngine;
using System;
using System.Collections;


public class Vector3XTweenProperty : AbstractVector3TweenProperty, IGenericProperty
{
	public string propertyName { get; private set; }
	protected Action<Vector3> _setter;
	protected Func<Vector3> _getter;
	
	protected new float _originalEndValue;
	protected new float _startValue;
	protected new float _endValue;
	protected new float _diffValue;
	
	
	public Vector3XTweenProperty( string propertyName, float endValue, bool isRelative = false )
	{
		this.propertyName = propertyName;
		_isRelative = isRelative;
		_originalEndValue = endValue;
	}
	
	
	/// <summary>
	/// validation checks to make sure the target has a valid property with an accessible setter
	/// </summary>
	public override bool validateTarget( object target )
	{
		// cache the setter
		_setter = GoTweenUtils.setterForProperty<Action<Vector3>>( target, propertyName );
		return _setter != null;
	}
	

	public override void prepareForUse()
	{
		// This might seem to be overkill, but on the case of Transforms that
		// have been destroyed, target == null will return false, whereas
		// target.Equals(null) will return true.  Otherwise we don't really
		// get the benefits of the nanny.
		if (_ownerTween.target == null || _ownerTween.target.Equals(null))
		{
			return;
		}

		// retrieve the getter
		_getter = GoTweenUtils.getterForProperty<Func<Vector3>>( _ownerTween.target, propertyName );
		
		_endValue = _originalEndValue;
		
		// if this is a from tween we need to swap the start and end values
		if( _ownerTween.isFrom )
		{
			_startValue = _endValue;
			_endValue = _getter().x;
		}
		else
		{
			_startValue = _getter().x;
		}

		// prep the diff value
		if( _isRelative && !_ownerTween.isFrom )
			_diffValue = _endValue;
		else
			_diffValue = _endValue - _startValue;
	}
	
	
	public override void tick( float totalElapsedTime )
	{
		var currentValue = _getter();
		currentValue.x = _easeFunction( totalElapsedTime, _startValue, _diffValue, _ownerTween.duration );
		
		_setter( currentValue );
	}

}
