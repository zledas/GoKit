using UnityEngine;
using System;
using System.Collections;


public sealed class FloatTweenProperty : AbstractTweenProperty, IGenericProperty
{
	public string propertyName { get; private set; }
	private Action<float> _setter;
	
	private float _originalEndValue;
	private float _startValue;
	private float _endValue;
	private float _diffValue;

	
	public FloatTweenProperty( string propertyName, float endValue, bool isRelative = false ) : base( isRelative )
	{
		this.propertyName = propertyName;
		_originalEndValue = endValue;
	}
	

	/// <summary>
	/// validation checks to make sure the target has a valid property with an accessible setter
	/// </summary>
	public override bool validateTarget( object target )
	{
		// cache the setter
		_setter = GoTweenUtils.setterForProperty<Action<float>>( target, propertyName );
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
		var getter = GoTweenUtils.getterForProperty<Func<float>>( _ownerTween.target, propertyName );
		
		_endValue = _originalEndValue;
		
		// if this is a from tween we need to swap the start and end values
		if( _ownerTween.isFrom )
		{
			_startValue = _endValue;
			_endValue = getter();
		}
		else
		{
			_startValue = getter();
		}
		
		// setup the diff value
		if( _isRelative && !_ownerTween.isFrom )
			_diffValue = _endValue;
		else
			_diffValue = _endValue - _startValue;
	}
	

	public override void tick( float totalElapsedTime )
	{
		var easedValue = _easeFunction( totalElapsedTime, _startValue, _diffValue, _ownerTween.duration );
		_setter( easedValue );
	}
}
