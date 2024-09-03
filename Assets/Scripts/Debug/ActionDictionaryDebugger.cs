using UnityEngine;

public class ActionDictionaryDebugger : MonoBehaviour
{
    AgentUtilityManager actionUtilityManager;
    Vector2 scrollPosition = Vector2.zero;

    private void Start()
    {
        actionUtilityManager = GetComponent<AgentUtilityManager>();
    }

    private void OnGUI()
    {
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 30,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft,
        };
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold,
        };
        GUIStyle boxStyle = new GUIStyle(GUI.skin.box)
        {
            normal = { background = MakeTex(2, 2, new Color(0, 0, 0, 0.5f)) },
        };

        float boxWidth = 800;
        float boxHeight = 800;

        GUILayout.BeginArea(new Rect(10, 10, boxWidth, boxHeight), boxStyle);
        GUILayout.BeginVertical();

        if (actionUtilityManager != null && actionUtilityManager.actions != null)
        {
            scrollPosition = GUILayout.BeginScrollView(
                scrollPosition,
                GUILayout.Width(boxWidth),
                GUILayout.Height(boxHeight)
            );

            foreach (var action in actionUtilityManager.actions)
            {
                float utilityScore = action.utilityScore;
                int cost = action.cost;

                GUIContent actionContent = new GUIContent($"Action: {action.name}");
                GUIContent utilityScoreContent = new GUIContent($"UtilityScore: {utilityScore:F2}");
                GUIContent costContent = new GUIContent($"Cost: {cost}");

                float maxWidth = Mathf.Max(
                    labelStyle.CalcSize(actionContent).x,
                    labelStyle.CalcSize(utilityScoreContent).x,
                    labelStyle.CalcSize(costContent).x
                );

                labelStyle.normal.textColor = Color.white;
                GUILayout.Label(actionContent, labelStyle, GUILayout.Width(maxWidth));

                labelStyle.normal.textColor = Color.magenta;
                GUILayout.Label(utilityScoreContent, labelStyle, GUILayout.Width(maxWidth));

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
            GUILayout.Label("Please assign the ActionUtilityManager component.", labelStyle);
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private void PrintProbabilitiesToConsole()
    {
        if (actionUtilityManager != null && actionUtilityManager.actions != null)
        {
            foreach (var action in actionUtilityManager.actions)
            {
                float utilityScore = action.utilityScore;
                int cost = action.cost;

                Debug.Log($"Action: {action.name}, UtilityScore: {utilityScore:F2}, Cost: {cost}");
            }
        }
        else
        {
            Debug.LogError("ActionUtilityManager or actions list is null.");
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
