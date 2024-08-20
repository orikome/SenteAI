using UnityEngine;

public class ActionDictionaryDebugger : MonoBehaviour
{
    AgentActionUtilityManager actionDictionary;
    Vector2 scrollPosition = Vector2.zero;

    private void Start()
    {
        actionDictionary = GetComponent<AgentActionUtilityManager>();
    }

    private void OnGUI()
    {
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 30,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft
        };
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold
        };
        GUIStyle boxStyle = new GUIStyle(GUI.skin.box)
        {
            normal = { background = MakeTex(2, 2, new Color(0, 0, 0, 0.5f)) }
        };

        float boxWidth = 800;
        float boxHeight = 800;

        GUILayout.BeginArea(new Rect(10, 10, boxWidth, boxHeight), boxStyle);
        GUILayout.BeginVertical();

        if (actionDictionary != null)
        {
            scrollPosition = GUILayout.BeginScrollView(
                scrollPosition,
                GUILayout.Width(boxWidth),
                GUILayout.Height(boxHeight)
            );

            foreach (var actionProbability in actionDictionary.utilityScore)
            {
                AgentAction action = actionProbability.Key;
                float weight = actionProbability.Value;
                int cost = action.cost;

                GUIContent actionContent = new GUIContent($"Action: {action.name}");
                GUIContent weightContent = new GUIContent($"Weight: {weight}");
                GUIContent costContent = new GUIContent($"Cost: {cost}");

                float maxWidth = Mathf.Max(
                    labelStyle.CalcSize(actionContent).x,
                    labelStyle.CalcSize(weightContent).x,
                    labelStyle.CalcSize(costContent).x
                );

                labelStyle.normal.textColor = Color.white;
                GUILayout.Label(actionContent, labelStyle, GUILayout.Width(maxWidth));

                labelStyle.normal.textColor = Color.magenta;
                GUILayout.Label(weightContent, labelStyle, GUILayout.Width(maxWidth));

                labelStyle.normal.textColor = Color.green;
                GUILayout.Label(costContent, labelStyle, GUILayout.Width(maxWidth));

                labelStyle.normal.textColor = Color.blue;
            }

            GUILayout.EndScrollView();

            if (GUILayout.Button("Print to Console", buttonStyle))
            {
                PrintProbabilitiesToConsole();
            }
        }
        else
        {
            labelStyle.normal.textColor = Color.red;
            GUILayout.Label("Please assign the ActionDictionary component.", labelStyle);
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private void PrintProbabilitiesToConsole()
    {
        foreach (var actionProbability in actionDictionary.utilityScore)
        {
            AgentAction action = actionProbability.Key;
            float weight = actionProbability.Value;
            int cost = action.cost;

            Debug.Log($"Action: {action.name}, Weight: {weight}, Cost: {cost}");
        }
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
