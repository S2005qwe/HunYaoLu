using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewMusicTrack", menuName = "MusicGame/MusicTrackData")]
public class NoteData_SO : ScriptableObject
{
    public List<NoteDat> allLanes;

    // 一键按下落时间排序
    public void SortAllNotes()
    {
        foreach (var lane in allLanes)
        {
            lane.laneNotes.Sort((a, b) => a.fallStartTime.CompareTo(b.fallStartTime));
        }
    }
}