using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
public struct DonationEntry
{
    [TextArea(2, 4)]
    public string donationText;
    public VideoClip videoClip;
}

public class DonationQueue : MonoBehaviour
{
    // Массив заполняется вручную в Inspector:
    // каждый элемент = один донат (текст + видеоклип)
    [SerializeField] private DonationEntry[] entries = new DonationEntry[]
    {
        new DonationEntry
        {
            donationText = "силвер ты лучший стример на этом сайте желаю тебе удачи и побольше зрителей ты делаешь отличный контент"
        },
        new DonationEntry
        {
            donationText = "привет с донатом от всей нашей компании мы смотрим тебя каждый стрим и очень рады что ты продолжаешь стримить"
        },
        new DonationEntry
        {
            donationText = "дорогой стример хочу сказать что твои стримы помогают мне расслабиться после тяжёлого рабочего дня спасибо тебе"
        },
        new DonationEntry
        {
            donationText = "силвер поздравляю с тысячей подписчиков это заслуженно ты вкладываешь душу в каждый стрим и это очень чувствуется"
        },
        new DonationEntry
        {
            donationText = "привет это мой первый донат но я смотрю тебя уже полгода наконец решился написать ты крутой продолжай в том же духе"
        },
        new DonationEntry
        {
            donationText = "хочу заказать песню если можно поставь что нибудь весёлое мы тут всей семьёй смотрим и хотим потанцевать"
        },
        new DonationEntry
        {
            donationText = "стример ты сегодня очень хорошо играешь это просто огонь мы в чате болеем за тебя не сдавайся и продолжай"
        },
        new DonationEntry
        {
            donationText = "залупа кал какшки чмоооо"
        },
    };

    private int currentIndex;

    public DonationEntry Current  => entries[currentIndex];
    public int           Count    => entries.Length;
    public int           Index    => currentIndex;

    public void Advance()
    {
        currentIndex = (currentIndex + 1) % entries.Length;
    }

    public void Reset()
    {
        currentIndex = 0;
    }
}
