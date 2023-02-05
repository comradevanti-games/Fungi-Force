using TMPro;
using UnityEngine;

namespace TeamShrimp.GGJ23
{
    public class TeamTextUpdater : MonoBehaviour
    {
        [SerializeField] private string template;
        [SerializeField] private TextMeshProUGUI displayLabel;


        private string Text
        {
            set => displayLabel.text = value;
        }


        public void OnTeamChanged(Team team)
        {
            DisplayTextFor(team);
        }

        private void DisplayTextFor(Team team)
        {
            Text = template.Replace("[Team]", team.ToString());
        }
    }
}