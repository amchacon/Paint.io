using System.Collections;
using UnityEngine;

public class TimeManager : SingletonMB<TimeManager>
{
	private string m_URL = "http://leatonm.net/wp-content/uploads/2017/candlepin/getdate.php";
	private string m_TimeData;
	private string m_CurrentTime;
	private string m_CurrentDate;

	void Start()
	{
		StartCoroutine(GetTime());
	}

	public IEnumerator GetTime()
	{
		WWW www = new WWW(m_URL);
		yield return www;
		if (www.error == null)
		{
			m_TimeData = www.text;
			string[] words = m_TimeData.Split('/');
			m_CurrentDate = words[0];
			m_CurrentTime = words[1];
		}
	}

	public int GetCurrentDateNow()
	{
		string[] words = m_CurrentDate.Split('-');
		int x = int.Parse(words[0] + words[1] + words[2]);
		return x;
	}

	public string GetCurrentTimeNow()
	{
		return m_CurrentTime;
	}
}