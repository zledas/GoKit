using UnityEngine;
using System;
using System.Collections;


public class Vector3XYTweenProperty : AbstractVector3TweenProperty, IGenericProperty
{
    public string propertyName { get; private set; }
    protected Action<Vector3> _setter;
    protected Func<Vector3> _getter;

    protected new Vector2 _originalEndValue;
    protected new Vector2 _startValue;
    protected new Vector2 _endValue;
    protected new Vector2 _diffValue;


    public Vector3XYTweenProperty(string propertyName, Vector2 endValue, bool isRelative = false)
    {
        this.propertyName = propertyName;
        _isRelative = isRelative;
        _originalEndValue = endValue;
    }


    /// <summary>
    /// validation checks to make sure the target has a valid property with an accessible setter
    /// </summary>
    public override bool validateTarget(object target)
    {
        // cache the setter
        _setter = GoTweenUtils.setterForProperty<Action<Vector3>>(target, propertyName);
        return _setter != null;
    }


    public override void prepareForUse()
    {
        if (_ownerTween.target == null)
        {
            return;
        }

        // retrieve the getter
        _getter = GoTweenUtils.getterForProperty<Func<Vector3>>(_ownerTween.target, propertyName);

        _endValue = _originalEndValue;

        // if this is a from tween we need to swap the start and end values
        if (_ownerTween.isFrom)
        {
            _startValue = _endValue;
            _endValue = new Vector2(_getter().x, _getter().y);
        }
        else
        {
            _startValue = new Vector2(_getter().x, _getter().y);
        }

        // prep the diff value
        if (_isRelative && !_ownerTween.isFrom)
            _diffValue = _endValue;
        else
            _diffValue = _endValue - _startValue;
    }


    public override void tick(float totalElapsedTime)
    {
        var currentValue = _getter();
        currentValue.x = _easeFunction(totalElapsedTime, _startValue.x, _diffValue.x, _ownerTween.duration);
        currentValue.y = _easeFunction(totalElapsedTime, _startValue.y, _diffValue.y, _ownerTween.duration);
        _setter(currentValue);
    }

}
