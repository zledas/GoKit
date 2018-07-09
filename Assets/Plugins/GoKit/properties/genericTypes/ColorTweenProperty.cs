using UnityEngine;
using System;
using System.Collections;


public sealed class ColorTweenProperty : AbstractMaterialColorTweenProperty, IGenericProperty
{
	public string propertyName { get; private set; }
	private Action<Color> _setter;


	public ColorTweenProperty( string propertyName, Color endValue, bool isRelative = false ) : base( endValue, isRelative )
	{
		this.propertyName = propertyName;
	}


	/// <summary>
	/// validation checks to make sure the target has a valid property with an accessible setter
	/// </summary>
	public override bool validateTarget( object target )
	{
		// cache the setter
		_setter = GoTweenUtils.setterForProperty<Action<Color>>( target, propertyName );
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
		var getter = GoTweenUtils.getterForProperty<Func<Color>>( _ownerTween.target, propertyName );
		
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
		
		base.prepareForUse();
	}
	
	
	public override void tick( float totalElapsedTime )
	{
		var easedTime = _easeFunction( totalElapsedTime, 0, 1, _ownerTween.duration );
		var newColor = GoTweenUtils.unclampedColorLerp( _startValue, _diffValue, easedTime );
		
		_setter( newColor );
	}

}
