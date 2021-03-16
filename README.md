# TJS Tween Job System
Optimized animation engine for Unity v1.0


**1- Shortcuts**
Shortcut extensions that directly extend common objects like this:
```cs
// Move a transform to position 100,0,0 in 2 second.
transform.TweenPosition(new Vector3(100, 0, 0), 2f).Play();

// Scale a transform to 2 in 1 second with ease BackEaseIn.
transform.TweenScale(new Vector3(2,2,2), 1).SetEase(EaseType.BackEaseIn).Play();
```

**2- Full Control**

- Rewind:
```cs
//FadeOut the target sprite renderer then playback the animation (fadeIn) by adding 'true' as parameter to Play method.
var sprite = GetComponent<SpriteRenderer>(); //target sprite renderer
sprite.TweenOpacity(0, 2f).Play(true);
```

- Pause / Resume / Kill
```cs
var sprite = GetComponent<SpriteRenderer>(); //get the target
var tween = sprite.TweenColor(Color.red, 1f).Play(); // tween the color to red in 1 sec.
            
//pause the animation
tween.Pause();
            
//resume the animation
tween.Resume();
            
 //stop & kill the animation
tween.Kill();
```

**3- Events**

```cs
var sprite = GetComponent<SpriteRenderer>(); //get the target
var tween = sprite.TweenColor(Color.red, 1f).Play(); // tween the color to red in 1 sec.
            
//On Complete event
tween.OnComplete((() => Debug.Log($"Animation Completed !")));
            
//On Kill event
tween.OnKill((() => Debug.Log($"Animation killed !")));
            
//On Rewind event
tween.OnRewind((() => Debug.Log($"Animation Rewinded !")));
            
//On Play event
tween.OnPlay((() => Debug.Log($"Animation Played !")));
            
//On Pause event
tween.OnPause((() => Debug.Log($"Animation Resumed !")));
            
//On Resume event
tween.OnResume((() => Debug.Log($"Animation Resumed !")));
            
//On Update event
tween.OnUpdate((data =>
 {
   Debug.Log($"Remaining Time : {data.timeRemaining}"); //the remaining time of the animation
   Debug.Log($"Normalized Time : {data.normalizedTime}"); //useful with progressBars or curves  
 }));
```
```cs
 //- chained methods example
 var sprite = GetComponent<SpriteRenderer>(); //get the target
sprite.TweenColor(Color.red, 1f)
      .OnComplete((() => Debug.Log($"Animation Completed !")))
      .OnKill((() => Debug.Log($"Animation Killed !")))
      .OnUpdate(OnUpdateAnimation)
      .Play();
```
