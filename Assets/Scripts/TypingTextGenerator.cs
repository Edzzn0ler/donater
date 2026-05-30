using UnityEngine;

public class TypingTextGenerator : MonoBehaviour
{
    [SerializeField] private string[] texts = {
        "силвер ты лучший стример на этом сайте желаю тебе удачи и побольше зрителей ты делаешь отличный контент",
        "привет с донатом от всей нашей компании мы смотрим тебя каждый стрим и очень рады что ты продолжаешь стримить",
        "дорогой стример хочу сказать что твои стримы помогают мне расслабиться после тяжёлого рабочего дня спасибо тебе",
        "силвер поздравляю с тысячей подписчиков это заслуженно ты вкладываешь душу в каждый стрим и это очень чувствуется",
        "привет это мой первый донат но я смотрю тебя уже полгода наконец решился написать ты крутой продолжай в том же духе",
        "хочу заказать песню если можно поставь что-нибудь весёлое мы тут всей семьёй смотрим и хотим потанцевать",
        "стример ты сегодня очень хорошо играешь это просто огонь мы в чате болеем за тебя не сдавайся и продолжай",
        "залупа кал какшки чмоооо",
    };

    private int[] order;
    private int currentIndex;

    private void Awake()
    {
        order = new int[texts.Length];
        for (int i = 0; i < texts.Length; i++) order[i] = i;
        Shuffle();
    }

    private void Shuffle()
    {
        for (int i = order.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (order[i], order[j]) = (order[j], order[i]);
        }
    }

    public string GetCurrentText() => texts[order[currentIndex % texts.Length]];

    public void Advance()
    {
        currentIndex++;
        if (currentIndex % texts.Length == 0)
            Shuffle();
    }
}
