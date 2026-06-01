using EFT;
using Softwyx.CareerLog.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Softwyx.CareerLog.Collectors.Movement;

internal static class MovementSampler{
    private static Coroutine _coroutine;
    private static Player    _player;

    public static void Start(Player player){
        Stop();

        if(!Settings.Enabled.Value || player == null || !player.IsYourPlayer) return;

        _player = player;
        MovementSampleBuffer.Clear();
        MovementSampleBuffer.Add(player.Position, RaidEventClock.ElapsedSeconds());

        _coroutine = MovementSamplerHost.Ensure().
                                         StartCoroutine(SampleLoop());
    }

    public static List<MovementSample> StopAndTakeSamples(){
        Stop();

        return MovementSampleBuffer.TakeAll();
    }

    public static void Stop(){
        if(_coroutine != null){
            MovementSamplerHost.Ensure().
                                StopCoroutine(_coroutine);
            _coroutine = null;
        }

        _player = null;
    }

    private static IEnumerator SampleLoop(){
        while(_player){
            var interval = Mathf.Max(0.4f, Settings.MovementSampleIntervalSec.Value);

            yield return new WaitForSeconds(interval);

            if(!_player || !_player.IsYourPlayer) yield break;

            MovementSampleBuffer.Add(_player.Position, RaidEventClock.ElapsedSeconds());
        }
    }
}
