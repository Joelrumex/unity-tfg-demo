using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class EndingSceneManager : MonoBehaviour
{
    public enum EndingType { Pacifist, Genocide }

    [Header("Ending Type")]
    public EndingType endingType;

    [Header("UI")]
    public CanvasGroup    canvasGroup;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public TextMeshProUGUI statsText;
    public Button         newGameButton;
    public Image          backgroundImage;

    [Header("Pacifist Settings")]
    public Color pacifistBackgroundColor = new Color(0.85f, 0.95f, 1f);
    public Color pacifistTitleColor      = new Color(0.3f, 0.6f, 1f);
    [TextArea] public string pacifistTitle = "True Pacifist";
    [TextArea] public string pacifistBody  =
        "You walked through war and chose not to become it.\n\n" +
        "Every enemy you faced, you offered something rarer than victory.\n\n" +
        "You offered them a way out.\n\n" +
        "The forest breathes again.\n" +
        "The knight remembers her name.\n" +
        "And the Red Sister — for the first time — is quiet.";

    [Header("Genocide Settings")]
    public Color genocideBackgroundColor = new Color(0.1f, 0.05f, 0.05f);
    public Color genocideTitleColor      = new Color(0.8f, 0.1f, 0.1f);
    [TextArea] public string genocideTitle = "No Mercy";
    [TextArea] public string genocideBody  =
        "You had every chance to stop.\n\n" +
        "The mage held a seed in his hand.\n" +
        "The knight just wanted her back.\n" +
        "Even she, at the end, was tired.\n\n" +
        "You gave none of them the chance to find out\n" +
        "what came after the anger.\n\n" +
        "The war is over.\n" +
        "But so is everything else.";

    [Header("References")]
    public BattleResult battleResult;
    public string       mainMenuScene = "MainMenu";

    [Header("Timing")]
    public float fadeInDuration  = 2f;
    public float textRevealDelay = 0.5f;   // Delay between title and body appearing

    void Start()
    {
        canvasGroup.alpha = 0f;
        newGameButton.gameObject.SetActive(false);
        statsText.gameObject.SetActive(false);

        newGameButton.onClick.AddListener(StartNewGame);

        ApplyEndingContent();
        StartCoroutine(PlayEnding());
    }

    void ApplyEndingContent()
    {
        if (endingType == EndingType.Pacifist)
        {
            titleText.text  = pacifistTitle;
            bodyText.text   = pacifistBody;
            titleText.color = pacifistTitleColor;
            if (backgroundImage != null)
                backgroundImage.color = pacifistBackgroundColor;
        }
        else
        {
            titleText.text  = genocideTitle;
            bodyText.text   = genocideBody;
            titleText.color = genocideTitleColor;
            if (backgroundImage != null)
                backgroundImage.color = genocideBackgroundColor;
        }

        // Show run stats
        statsText.text =
            $"Enemies defeated: {battleResult.TotalKills}\n" +
            $"Enemies redeemed: {battleResult.TotalMercies}";
    }

    IEnumerator PlayEnding()
    {
        // Fade in slowly
        yield return StartCoroutine(Fade(0f, 1f, fadeInDuration));

        // Typewrite the body text
        yield return StartCoroutine(TypewriteText(bodyText));

        // Show stats and button after text finishes
        yield return new WaitForSeconds(textRevealDelay);
        statsText.gameObject.SetActive(true);

        yield return new WaitForSeconds(textRevealDelay);
        newGameButton.gameObject.SetActive(true);
    }

    IEnumerator TypewriteText(TextMeshProUGUI tmp)
    {
        string fullText = tmp.text;
        tmp.text = "";

        foreach (char c in fullText)
        {
            tmp.text += c;
            // Slower on punctuation for dramatic effect
            float delay = (c == '.' || c == '\n') ? 0.08f : 0.03f;
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = to;
    }

    void StartNewGame()
    {
        battleResult.FullReset();
        StartCoroutine(FadeAndLoad(mainMenuScene));
    }

    IEnumerator FadeAndLoad(string sceneName)
    {
        yield return StartCoroutine(Fade(1f, 0f, 0.5f));
        SceneManager.LoadScene(sceneName);
    }
}