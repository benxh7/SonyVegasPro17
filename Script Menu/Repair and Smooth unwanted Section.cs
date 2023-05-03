/*****************************************************
 * This script automatically cover a jump cut edit and make it look smooth by using the 'VEGAS Warp Flow' transition so the viewer does not notice the cut at all.
 * Date: 2019/07/05
 * Authors: Pascal Kelm
 *          Dee Fussell 
 * Requirement: VEGAS Version > 17.0 ('Warp Flow' transition)
 ****************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using ScriptPortal.Vegas;


public class EntryPoint
{
    Vegas myVegas;

    /// <summary>
    /// Name of the transition
    /// </summary>
    public string transitionName = "VEGAS Warp Flow";

    /// <summary>
    /// Split length in frames. A value smaller than 0 creates a split in the middle of the selected area.
    /// </summary>
    public const double splitLength = 5.0; // in frames

    public void FromVegas(Vegas vegas)
    {
        /*splitLength = length;*/
        myVegas = vegas;       
        
        // No region selected
        if (myVegas.SelectionLength.FrameCount == 0)
        {
            MessageBox.Show(Vegas.LRZ("VegasUtilities.SmartEdit.Warning.NoRegionSelected.Text"), Vegas.LRZ("VegasUtilities.SmartEdit.Warning.NoRegionSelected.Title"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Get the selected track
        VideoEvent selectedTrackEvent = (VideoEvent)FindFirstSelectedEventUnderCursor();
        // No track selected
        if (selectedTrackEvent == null)
        {
            MessageBox.Show(Vegas.LRZ("VegasUtilities.SmartEdit.Warning.NoTrackSelected.Text"), Vegas.LRZ("VegasUtilities.SmartEdit.Warning.NoTrackSelected.Title"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Get the transition
        PlugInNode transitionNode = vegas.Transitions.GetChildByName(transitionName);        
        if (transitionNode == null)
        {
            MessageBox.Show(String.Format(Vegas.LRZ("VegasUtilities.SmartEdit.Warning.NoTransition.Text"), transitionName), Vegas.LRZ("VegasUtilities.SmartEdit.Warning.NoTransition.Text"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // One side of the selection is out the selected clip 
        if (myVegas.SelectionStart < selectedTrackEvent.Start ||
            myVegas.SelectionStart + myVegas.SelectionLength > selectedTrackEvent.End)
        {
            DialogResult dialogResult = MessageBox.Show(Vegas.LRZ("VegasUtilities.SmartEdit.Warning.SelectionIsOutOfRange.Text"), Vegas.LRZ("VegasUtilities.SmartEdit.Warning.SelectionIsOutOfRange.Title"), MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                using (new UndoBlock(myVegas.Project, Vegas.LRZ("VegasUtilities.SmartEdit.Undo.SelectionIsOutOfRange.Text")))
                {
                    if (myVegas.SelectionStart < selectedTrackEvent.Start)
                        myVegas.SelectionStart = selectedTrackEvent.Start;
                    if (myVegas.SelectionStart + myVegas.SelectionLength > selectedTrackEvent.End)
                        myVegas.SelectionLength = selectedTrackEvent.End - myVegas.SelectionStart;
                }
            }
            else if (dialogResult == DialogResult.No)
            {
                return;
            }
        }

        using (new UndoBlock(myVegas.Project, Vegas.LRZ("VegasUtilities.RepairAndSmoothUnwantedSection.Undo.Text")))
        {
            // convert frames to milliseconds
            double splitLength_ms = (splitLength * 1000.0) / selectedTrackEvent.Length.FrameRate;
            if (SplitTrackAndRemoveMiddle(selectedTrackEvent, splitLength_ms))
            {
                Effect transition = new Effect(transitionNode);
                CreateTransition(selectedTrackEvent, transition);
            }
        }
    }
         
    /// <summary>
    /// Returns the first selected event that's under the cursor
    /// </summary>
    /// <returns>The first selected event or null if no event selected</returns>
    private TrackEvent FindFirstSelectedEventUnderCursor()
    {
        foreach (Track track in myVegas.Project.Tracks)
        {
            foreach (TrackEvent trackEvent in track.Events)
            {
                if (trackEvent.Selected && trackEvent.Start <= myVegas.Transport.CursorPosition && trackEvent.End >= myVegas.Transport.CursorPosition)
                {
                    return trackEvent;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Split the selected area in the middle and remove this part 
    /// </summary>
    /// <param name="trackEvent">Track to split.</param>
    /// <param name="splitLength">Length of the split in milliseconds</param>
    /// <returns>Returns true, if the split succeeded.</returns>
    private bool SplitTrackAndRemoveMiddle(TrackEvent trackEvent, double splitLength)
    {
        // split the selected area in the middle
        // check: Is the user offset bigger than the selection length?
        if (splitLength <= 0 ||
           (2.0 * splitLength) > myVegas.SelectionLength.ToMilliseconds())
        {
            splitLength = myVegas.SelectionLength.ToMilliseconds() / 2.0;
            Timecode splitMiddle = new Timecode(splitLength);
            Timecode splitStart = (myVegas.SelectionStart + splitMiddle) - trackEvent.Start;

            if (trackEvent.IsGrouped)
            {
                int groupSize = trackEvent.Group.Count;
                // split audio and video in the group
                // foreach is impossibel because Split is dynamic increasing the groupSize
                for (int i = 0; i < groupSize; i++)
                {
                   trackEvent.Group[i].Split(splitStart);
                }

                SplitGroup(trackEvent, groupSize);
            }
            else
            {
                trackEvent.Split(splitStart);
            }
        }
        // split the selected area with an offset
        else
        {
            if (trackEvent.IsGrouped)
            {
                int groupSize = trackEvent.Group.Count;
                // split audio and video in the group
                // foreach loop is impossibel because Split() is dynamic increasing the groupSize
                for (int i = 0; i < groupSize; i++)
                {
                    RoutineSplitAndRemove(trackEvent.Group[i], splitLength);
                }

                SplitGroup(trackEvent, groupSize);
            }
            else
            {
                return RoutineSplitAndRemove(trackEvent, splitLength);
            }
        }
        return true;
    }

    /// <summary>
    /// Split the group of the selected track event into new two.
    /// </summary>
    /// <param name="trackEvent">Track event which includes the group to split.</param>
    /// <param name="groupSize">Size of the group.</param>
    private void SplitGroup(TrackEvent trackEvent, int groupSize)
    {
        TrackEventGroup groupNew = new TrackEventGroup(myVegas.Project);
        myVegas.Project.TrackEventGroups.Add(groupNew);
        TrackEventGroup groupOld = trackEvent.Group;
        for (int i = 0; i < groupSize; i++)
        {
            groupNew.Add(groupOld[0]);
        }
    }

    /// <summary>
    /// Split the selected area in the middle and remove this part 
    /// </summary>
    /// <param name="trackEvent">Track to split.</param>
    /// <param name="splitLength">Length of the split in milliseconds</param>
    /// <returns>Returns true, if the split succeeded.</returns>
    private bool RoutineSplitAndRemove(TrackEvent trackEvent, double splitLength)
    {
        Timecode splitMiddleStart = myVegas.SelectionStart + new Timecode(splitLength);
        Timecode splitMiddleEnd = splitMiddleStart + myVegas.SelectionLength - new Timecode(2 * splitLength);

        trackEvent.Split(splitMiddleEnd - trackEvent.Start);
        TrackEvent middleTrack = trackEvent.Split(splitMiddleStart - trackEvent.Start);
        if (middleTrack != null)
        { 
            middleTrack.Track.Events.RemoveAt(middleTrack.Index);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Create a given transition for the overlapping area.
    /// </summary>
    /// <param name="trackEvent">Selected track event.</param>
    /// <param name="transition">Transition effect. Default: "VEGAS Warp Flow" Transition.</param>
    private void CreateTransition(TrackEvent trackEvent, Effect transition)
    {
        if (trackEvent.IsGrouped)
        {
            for (int i = 0; i < trackEvent.Group.Count; i++)
            {
                Track currentTrack = trackEvent.Group[i].Track;
                TrackEvents trackEvents = currentTrack.Events;
                TrackEvent trackItem = trackEvents[trackEvent.Group[i].Index + 1];

                Timecode itemLength = trackEvents[trackEvent.Group[i].Index + 1].Length;
                trackItem.Start = myVegas.SelectionStart;
                trackItem.End = trackItem.Start + itemLength;

                // add video transition
                if (trackItem.MediaType == MediaType.Video)
                {
                    trackItem.FadeIn.Transition = transition;
                }
            }
        }
        else
        {
            Track currentTrack = trackEvent.Track;
            TrackEvents trackEvents = currentTrack.Events;
            int position = trackEvent.Index;
            TrackEvent trackItem = trackEvents[position + 1];

            Timecode itemLength = trackEvents[position + 1].Length;
            trackItem.Start = myVegas.SelectionStart;
            trackItem.End = trackItem.Start + itemLength;

            // add video transition
            if (trackItem.MediaType == MediaType.Video)
            {
                trackItem.FadeIn.Transition = transition;
            }
        }
    }
}
