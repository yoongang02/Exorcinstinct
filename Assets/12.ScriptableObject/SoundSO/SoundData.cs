using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sound Data", menuName = "Sound/Create Sound Data")]
public class SoundData : ScriptableObject
{
    public string soundName; //sound 아이디
    public AudioClip clip;  //sound clip
    public int loopCnt; //루프 횟수
    public float volume; //볼륨
}