using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.XR.Interaction.Toolkit;

public class TimelinePartInstallerManager : MonoBehaviour
{
    [System.Serializable]
    public class InstallStep
    {
        public string partName; // e.g., "CPU", "RAM"
        public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket;
    }

    public PlayableDirector timeline;
    public List<InstallStep> installSteps = new List<InstallStep>();

    private int currentStepIndex = -1;
    private bool waiting = false;

    void Start()
    {
        if (timeline != null)
        {
            timeline.Play();
        }
    }

    // Called from Timeline Signal Receiver
    public void WaitForNextPart()
    {
        currentStepIndex++;


        InstallStep step = installSteps[currentStepIndex];


        waiting = true;
        timeline.Pause();
        step.socket.selectEntered.AddListener(OnPartSocketed);
    }

    private void OnPartSocketed(SelectEnterEventArgs args)
    {
        if (!waiting) return;

        GameObject attached = args.interactableObject.transform.gameObject;
        InstallStep step = installSteps[currentStepIndex];


        if (attached.name.Contains(step.partName)) // or check tag
        {
            waiting = false;
            step.socket.selectEntered.RemoveListener(OnPartSocketed);
            timeline.Resume();
        }
    }
}
