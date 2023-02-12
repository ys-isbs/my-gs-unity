using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerNameLabel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] RectTransform labelRectTransform;
    private PlayerController targetPlayer;
    private Transform labelTarget;
    private Camera cam;
    private Renderer targetRenderer;

    public void Init(PlayerController player, Transform target)
    {
        targetPlayer = player;
        nameText.text = player.Name;
        labelTarget = target;
        cam = Camera.main;
        targetRenderer = player.GetComponentInChildren<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (targetPlayer == null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            labelRectTransform.position = cam.WorldToScreenPoint(labelTarget.position);
            nameText.gameObject.SetActive(targetRenderer.isVisible);
        }
    }
}
