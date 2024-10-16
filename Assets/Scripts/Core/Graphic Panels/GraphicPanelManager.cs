using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicPanelManager : MonoBehaviour
{
    public static GraphicPanelManager instance { get; private set; }
    public const float DEAULT_TRANSITION_SPEED = 3f;
    [SerializeField] private GraphicPanel[] allPanels;
    private void Awake()
    {
        instance = this;
    }
    public GraphicPanel GetPanel(string name)
    {
        name=name.ToLower();
        foreach (var panel in allPanels)
        {
            if(panel.panelName.ToLower()==name)
                return panel;
        }
        return null;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
