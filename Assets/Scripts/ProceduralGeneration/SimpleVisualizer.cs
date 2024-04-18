using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SimpleVisualizer : MonoBehaviour
{
    public LSystemGenerator lSystem;
    List<Vector3> positions = new List<Vector3>();
    public GameObject prefab;
    public Material lineMaterial;

    [SerializeField] private int length = 8;
    [SerializeField] private float angle = 80;

	public int Length 
    { 
        get
        {
            if (length > 0) return length;
            else return 1;
        }
        set => length = value; 
    }

	private void Start()
	{
        var sequence = lSystem.GenerateSentence();
        VisualizeSequence(sequence);
    }

	//private void Update()
	//{
	//	if(Input.GetKeyDown(KeyCode.Space))
 //       {
	//		var sequence = lSystem.GenerateSentence();
	//		VisualizeSequence(sequence);
	//	}
	//}

	private void VisualizeSequence(string sequence)
    {
        Stack<AgentParameters> savePoints = new Stack<AgentParameters>();
        var currentPosition = Vector3.zero;

        Vector3 direction = Vector3.up; // .forward in 3d
        Vector3 tempPosition = Vector3.zero;

        positions.Add(currentPosition);

        foreach(var letter in sequence)
        {
            EncodingLetters encoding = (EncodingLetters)letter;
			switch (encoding)
			{
				case EncodingLetters.save:
                    savePoints.Push(new AgentParameters
                    {
                        position = currentPosition,
                        direction = direction,
                        length = Length
                    });
					break;
				case EncodingLetters.load:
                    if(savePoints.Count > 0)
                    {
                        var agentParameters = savePoints.Pop();
                        currentPosition = agentParameters.position;
                        direction = agentParameters.direction;
                        Length = agentParameters.length;
                    }
                    else
                    {
                        throw new System.Exception("No saved points in stack!");
                    }
					break;
				case EncodingLetters.draw:
                    tempPosition = currentPosition;
                    currentPosition += direction * length;
                    DrawLine(tempPosition, currentPosition, Color.red);
                    //Length -= 2; // cause next line to be shorter
                    positions.Add(currentPosition);
					break;
				case EncodingLetters.turnRight:
                    direction = Quaternion.AngleAxis(angle, Vector3.forward) * direction; // forward acts as up / out of screen in top down 2d
					break;
				case EncodingLetters.turnLeft:
					direction = Quaternion.AngleAxis(-angle, Vector3.forward) * direction;
					break;
                default:
                    break;
			}
		}

        foreach(var position in positions)
        {
            Instantiate(prefab, position, Quaternion.identity);
        }
	}

	private void DrawLine(Vector3 start, Vector3 end, Color color)
	{
		GameObject line = new GameObject("line");
        line.transform.position = start;
        var lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
	}

	public enum EncodingLetters
    {
        unknown = '1',
        save = '[',
        load = ']',
        draw = 'F',
        turnRight = '+',
		turnLeft = '-',
    }
}


