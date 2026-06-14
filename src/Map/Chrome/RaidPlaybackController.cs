using System;
using System.Collections;
using Softwyx.CareerLog.Map.Data;
using Softwyx.CareerLog.Map.Markers;
using Softwyx.CareerLog.Map.Panel;
using Softwyx.CareerLog.Map.Viewport;
using UnityEngine;

namespace Softwyx.CareerLog.Map.Chrome;

internal sealed class RaidPlaybackController : MonoBehaviour{
    private TrailContext      _ctx;
    private RaidMovementIndex _index;
    private Coroutine         _tick;
    private TrailPresenter    _trail;
    private ViewportZoom      _zoom;

    public float CurrentTime{
        get;
        private set;
    }
    public float Duration => _index?.DurationSec ?? 0f;
    public bool IsPlaying{
        get;
        private set;
    }
    public float Speed{
        get;
        private set;
    } = PlaybackSpeeds.Default;
    public Visibility Visibility{
        get;
        private set;
    } = new();

    public event Action StateChanged;

    public void Bind(TrailPresenter trail, TrailContext ctx, ViewportZoom zoom, RaidMovementIndex index){
        StopPlayback();
        _trail      = trail;
        _ctx        = ctx;
        _zoom       = zoom;
        _index      = index;
        Visibility  = FilterPrefs.Load();
        CurrentTime = Duration;
        _trail?.ResetPlaybackDraw(_ctx);
        ApplyFrame();
        StateChanged?.Invoke();
    }

    public void PersistMarkerPrefs(){
        if(Visibility?.RememberPrefs == true)
            FilterPrefs.Save(Visibility);
        else
            FilterPrefs.ClearSaved();
    }

    public void Rewind(){
        Pause();
        Seek(0f);
    }

    public void Unbind(){
        StopPlayback();
        _trail = null;
        _ctx   = null;
        _zoom  = null;
        _index = null;
    }

    public void Seek(float seconds){
        var wasBefore = seconds + 0.05f < CurrentTime;
        CurrentTime = Mathf.Clamp(seconds, 0f, Duration);

        if(wasBefore || CurrentTime <= 0.01f) _trail?.ResetPlaybackDraw(_ctx);

        _ctx?.PopoverHost?.Hide();
        ApplyFrame();
        StateChanged?.Invoke();
    }

    public void SeekNormalized(float normalized){
        Seek(Mathf.Clamp01(normalized) * Duration);
    }

    public void TogglePlay(){
        if(IsPlaying)
            Pause();
        else
            Play();
    }

    public void Play(){
        if(Duration <= 0f || _index == null) return;

        if(CurrentTime >= Duration - 0.05f){
            CurrentTime = 0f;
            _trail?.ResetPlaybackDraw(_ctx);
        }

        IsPlaying = true;
        _tick     = StartCoroutine(Tick());
        StateChanged?.Invoke();
    }

    public void Pause(){
        IsPlaying = false;

        if(_tick != null){
            StopCoroutine(_tick);
            _tick = null;
        }

        StateChanged?.Invoke();
    }

    public void SetSpeed(float speed){
        Speed = speed;
        StateChanged?.Invoke();
    }

    public void SetVisibility(Visibility visibility){
        Visibility = visibility ?? new Visibility();

        if(Visibility.RememberPrefs) FilterPrefs.Save(Visibility);

        ApplyFrame();
        StateChanged?.Invoke();
    }

    public void Restart(){
        _trail?.ResetPlaybackDraw(_ctx);
        Seek(0f);
        Play();
    }

    public void StopPlayback(){
        Pause();
    }

    private IEnumerator Tick(){
        while(IsPlaying){
            var dt = Time.unscaledDeltaTime * Speed;

            if(CurrentTime >= Duration){
                CurrentTime = Duration;
                ApplyFrame();
                Pause();

                yield break;
            }

            CurrentTime = Mathf.Min(Duration, CurrentTime + dt);
            ApplyFrame();
            StateChanged?.Invoke();

            yield return null;
        }
    }

    private void ApplyFrame(){
        if(_trail == null || _ctx == null || _index == null) return;

        var markers = _index.MarkersVisibleAt(CurrentTime, Visibility);
        _trail.ApplyFrame(_ctx, _zoom, CurrentTime, markers);
    }
}
