using UnityEngine;
using System.Collections;


public sealed class ScaleTweenProperty : AbstractVector3TweenProperty
{
	public ScaleTweenProperty( Vector3 endValue, bool isRelative = false ) : base( endValue, isRelative )
	{}
	
	
	#region Object overrides
	
	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
	
	
	public override bool Equals( object obj )
	{
		// if base already determined that we are equal no need to check further
		if( base.Equals( obj ) )
			return true;
		
		// we can be equal if the other object is a ScalePathTweenProperty
		return obj.GetType() == typeof( ScalePathTweenProperty );
	}
	
	#endregion
	
	
	public override void prepareForUse()
	{
		_target = _ownerTween.target as Transform;
		// This might seem to be overkill, but on the case of Transforms that
		// have been destroyed, target == null will return false, whereas
		// target.Equals(null) will return true.  Otherwise we don't really
		// get the benefits of the nanny.
		if (_target == null || _target.Equals(null))
		{
			return;
		}

		_endValue = _originalEndValue;
		
		// if this is a from tween we need to swap the start and end values
		if( _ownerTween.isFrom )
		{
			_startValue = _endValue;
			_endValue = _target.localScale;
		}
		else
		{
			_startValue = _target.localScale;
		}
		
		base.prepareForUse();
	}
	
	
	public override void tick( float totalElapsedTime )
	{
		var easedTime = _easeFunction( totalElapsedTime, 0, 1, _ownerTween.duration );
		_target.localScale = GoTweenUtils.unclampedVector3Lerp( _startValue, _diffValue, easedTime );
	}

}
