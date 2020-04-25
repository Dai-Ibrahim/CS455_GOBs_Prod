using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Goal
{
    public string name;
    public float value;
    public float getDiscontentment(float newValue)
    {
        return newValue * newValue;
    }
    public Goal (string goalName, float goalValue)
    {
        name = goalName;
        value = goalValue;
    }
}

public class Action
{
    public string name;
    public List<Goal> targetGoals;
    public Action (string actionName)
    {
        name = actionName;
        targetGoals = new List<Goal>();
    }
    public float getGoalChange(Goal goal)
    {
        foreach (Goal target in targetGoals)
        {
            if (target.name == goal.name)
            {
                return target.value;
            }
        }
        return 0f;
    }
}

public class DecisonMaker : MonoBehaviour
{
    Goal[] goals;
    Action[] actions;
    Action changeOverTime;
    const float TICK_LENGTH = 5.0f;
	
	public Text DiscontentmentText;
	public Text EatText;
	public Text SleepText;
	public Text BathroomText;
	public Text ShowerText;
	public Text BoredomText;
	public Text ActionText;


    void Start()
    {
        goals = new Goal[5];
        goals[0] = new Goal("Eat", 4);
        goals[1] = new Goal("Sleep", 3);
        goals[2] = new Goal("Bathroom", 3);
		goals[3] = new Goal("Shower", 3);
		goals[4] = new Goal("Boredom", 3);

        actions = new Action[7];

        actions[0] = new Action("Take cat on a walk");
        actions[0].targetGoals.Add(new Goal("Eat", +1f));
        actions[0].targetGoals.Add(new Goal("Sleep", +2f));
        actions[0].targetGoals.Add(new Goal("Bathroom", +2f));
		actions[0].targetGoals.Add(new Goal("Shower", +3f));
        actions[0].targetGoals.Add(new Goal("Boredom", -4f));
		
		actions[1] = new Action("Let cat take a nap");
        actions[1].targetGoals.Add(new Goal("Eat", +1f));
        actions[1].targetGoals.Add(new Goal("Sleep", -4f));
        actions[1].targetGoals.Add(new Goal("Bathroom", 0f));
		actions[1].targetGoals.Add(new Goal("Shower", 0f));
        actions[1].targetGoals.Add(new Goal("Boredom", 0f));
		
		actions[2] = new Action("Give cat a bubble bath");
        actions[2].targetGoals.Add(new Goal("Eat", +2f));
        actions[2].targetGoals.Add(new Goal("Sleep", +1f));
        actions[2].targetGoals.Add(new Goal("Bathroom", 0f));
		actions[2].targetGoals.Add(new Goal("Shower", -4f));
        actions[2].targetGoals.Add(new Goal("Boredom", -3f));
		
		actions[3] = new Action("Give cat tuna");
        actions[3].targetGoals.Add(new Goal("Eat", -4f));
        actions[3].targetGoals.Add(new Goal("Sleep", 0f));
        actions[3].targetGoals.Add(new Goal("Bathroom", +1f));
		actions[3].targetGoals.Add(new Goal("Shower", 0f));
        actions[3].targetGoals.Add(new Goal("Boredom", 0f));
			
		actions[4] = new Action("Give cat milk");
        actions[4].targetGoals.Add(new Goal("Eat", -2f));
        actions[4].targetGoals.Add(new Goal("Sleep", 0f));
        actions[4].targetGoals.Add(new Goal("Bathroom", +4f));
		actions[4].targetGoals.Add(new Goal("Shower", 0f));
        actions[4].targetGoals.Add(new Goal("Boredom", -2f));
					
		actions[5] = new Action("Take cat to the litter box");
        actions[5].targetGoals.Add(new Goal("Eat", 0f));
        actions[5].targetGoals.Add(new Goal("Sleep", 0f));
        actions[5].targetGoals.Add(new Goal("Bathroom", -4f));
		actions[5].targetGoals.Add(new Goal("Shower", +1f));
        actions[5].targetGoals.Add(new Goal("Boredom", 0f));
		
		actions[6] = new Action("Pet cat");
        actions[6].targetGoals.Add(new Goal("Eat", 0f));
        actions[6].targetGoals.Add(new Goal("Sleep", -4f));
        actions[6].targetGoals.Add(new Goal("Bathroom", 0f));
		actions[6].targetGoals.Add(new Goal("Shower", 0f));
        actions[6].targetGoals.Add(new Goal("Boredom", -3f));

        changeOverTime = new Action("TikTok");
        changeOverTime.targetGoals.Add(new Goal("Eat", +4f));
        changeOverTime.targetGoals.Add(new Goal("Sleep", +1f));
        changeOverTime.targetGoals.Add(new Goal("Bathroom", +2f));
        changeOverTime.targetGoals.Add(new Goal("Shower", +1f));
        changeOverTime.targetGoals.Add(new Goal("Boredom", +2f));

        InvokeRepeating("Tick", 0f, TICK_LENGTH);
    }

    void Tick()
    {
        foreach (Goal goal in goals)
        {
            goal.value += changeOverTime.getGoalChange(goal);
            goal.value = Mathf.Max(goal.value, 0);
            
        }
		updateUIValues();
        Debug.Log("Ticking.");
    }
	
	void updateUIValues()
	{
		DiscontentmentText.text = CurrentDiscontentment().ToString();
		EatText.text = (goals[0].value).ToString();
		SleepText.text = goals[1].value.ToString();
		BathroomText.text = goals[2].value.ToString();
		ShowerText.text = goals[3].value.ToString();
		BoredomText.text = goals[4].value.ToString();

	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Action bestThingToDo = ChooseAction(actions,goals);
			ActionText.text = bestThingToDo.name.ToString();

            foreach (Goal goal in goals)
            {
                goal.value += bestThingToDo.getGoalChange(goal);
                goal.value = Mathf.Max(goal.value, 0);
                Debug.Log(goal.name + ": " + goal.value);
            }
        }
    }
    public Action ChooseAction(Action[] actions, Goal[] goals)
    {
        Action bestAction = null;
        float bestValue = Mathf.Infinity;

        foreach (Action thisAction in actions)
        {
            float thisValue = Discontentment(thisAction, goals);
            if (thisValue < bestValue)
            {
                bestValue = thisValue;
                bestAction = thisAction;
            }
        }
        return bestAction;
    }

    float Discontentment(Action action, Goal[] goals)
    {
        float discontentment = 0;
        foreach (Goal goal in goals)
        {
            float newValue = goal.value + action.getGoalChange(goal);
            newValue = Mathf.Max(newValue, 0);
            discontentment += goal.getDiscontentment(newValue);
        }
        return discontentment;
    }
    float CurrentDiscontentment()
    {
        float total = 0f;
        foreach(Goal goal in goals)
        {
            total += (goal.value * goal.value);
        }
        return total;
    }
}