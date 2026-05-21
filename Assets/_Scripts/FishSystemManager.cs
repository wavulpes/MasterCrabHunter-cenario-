using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishSystemManager : MonoBehaviour
{
    public static FishSystemManager Instance { get; private set; }

    [System.Serializable]
    public class FishOption
    {
        public string nome;
        public FishClassificacao classificacao;
    }

    public enum FishClassificacao
    {
        Normal,
        Venenoso,
        EmExtincao
    }

    [Header("Gameplay")]
    public float tempoDeRonda = 90f;
    public int pontosPorMulta = 20;
    public int penalidadePorPeixeNormal = 15;
    public List<FishOption> peixesDisponiveis = new List<FishOption>();

    [Header("Referências de UI (opcional)")]
    public Canvas uiCanvas;

    private GameObject popup;
    private Text textoStatus;
    private Text textoPontuacao;
    private Text textoTimer;
    private FishOption peixeSelecionado;
    private bool uiCriada;
    private int pontuacao;
    private float tempoRestante;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (peixesDisponiveis.Count == 0)
        {
            peixesDisponiveis.Add(new FishOption { nome = "Baiacu", classificacao = FishClassificacao.Venenoso });
            peixesDisponiveis.Add(new FishOption { nome = "Mero", classificacao = FishClassificacao.EmExtincao });
            peixesDisponiveis.Add(new FishOption { nome = "Tilápia", classificacao = FishClassificacao.Normal });
        }

        tempoRestante = tempoDeRonda;
    }

    private void Start()
    {
        CriarUIRuntimeSeNecessario();
        AtualizarHUD();
        FecharPopup();
    }

    private void Update()
    {
        if (tempoRestante > 0f)
        {
            tempoRestante -= Time.deltaTime;
            if (tempoRestante < 0f) tempoRestante = 0f;
        }
        AtualizarHUD();
    }

    public void AbrirPopupEscolhaPeixe()
    {
        CriarUIRuntimeSeNecessario();
        popup.SetActive(true);
        RecriarBotoesPeixe();
    }

    public void FecharPopup()
    {
        if (popup != null) popup.SetActive(false);
    }

    public bool PodeEntregarMulta()
    {
        return peixeSelecionado != null && tempoRestante > 0f;
    }

    public void EntregarMulta()
    {
        if (!PodeEntregarMulta()) return;

        if (peixeSelecionado.classificacao == FishClassificacao.Normal)
            pontuacao -= penalidadePorPeixeNormal;
        else
            pontuacao += pontosPorMulta;

        textoStatus.text = $"Multa entregue: {peixeSelecionado.nome} ({peixeSelecionado.classificacao})";
        peixeSelecionado = null;
        AtualizarHUD();
    }

    private void SelecionarPeixe(FishOption peixe)
    {
        peixeSelecionado = peixe;
        textoStatus.text = $"Peixe selecionado: {peixe.nome} ({peixe.classificacao})";
        FecharPopup();
        AtualizarHUD();
    }

    private void AtualizarHUD()
    {
        if (!uiCriada) return;
        textoPontuacao.text = $"Pontuação: {pontuacao}";

        int segundos = Mathf.CeilToInt(tempoRestante);
        textoTimer.text = $"Tempo: {segundos}s";

        if (tempoRestante <= 0f)
            textoStatus.text = "Tempo esgotado!";
    }

    private void CriarUIRuntimeSeNecessario()
    {
        if (uiCriada) return;

        if (uiCanvas == null)
        {
            GameObject canvasObj = new GameObject("GameUI");
            uiCanvas = canvasObj.AddComponent<Canvas>();
            uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        textoPontuacao = CriarTexto("PontuacaoText", new Vector2(20, -20), 20);
        textoTimer = CriarTexto("TimerText", new Vector2(20, -50), 20);
        textoStatus = CriarTexto("StatusText", new Vector2(20, -80), 18);

        popup = CriarPainel("PopupEscolhaPeixe", new Vector2(450, 360));
        popup.SetActive(false);

        var titulo = CriarTexto("TituloPopup", new Vector2(20, -20), 22, popup.transform);
        titulo.text = "Escolha o peixe para multar";

        var fecharBtn = CriarBotao("FecharBtn", "Fechar", new Vector2(-80, 20), popup.transform, FecharPopup);
        fecharBtn.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0);
        fecharBtn.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0);

        uiCriada = true;
    }

    private void RecriarBotoesPeixe()
    {
        for (int i = popup.transform.childCount - 1; i >= 0; i--)
        {
            var child = popup.transform.GetChild(i);
            if (child.name.StartsWith("FishBtn_")) Destroy(child.gameObject);
        }

        for (int i = 0; i < peixesDisponiveis.Count; i++)
        {
            FishOption peixe = peixesDisponiveis[i];
            float y = -70 - (i * 55);
            var btn = CriarBotao($"FishBtn_{i}", $"{peixe.nome} ({peixe.classificacao})", new Vector2(20, y), popup.transform, () => SelecionarPeixe(peixe));
            var rt = btn.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(400, 45);
        }
    }

    private GameObject CriarPainel(string nome, Vector2 tamanho)
    {
        GameObject panelObj = new GameObject(nome);
        panelObj.transform.SetParent(uiCanvas.transform, false);

        var image = panelObj.AddComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0.8f);

        var rt = panelObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = tamanho;

        return panelObj;
    }

    private Text CriarTexto(string nome, Vector2 anchoredPos, int fontSize, Transform parent = null)
    {
        GameObject textObj = new GameObject(nome);
        textObj.transform.SetParent(parent == null ? uiCanvas.transform : parent, false);

        var text = textObj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.color = Color.white;
        text.alignment = TextAnchor.UpperLeft;

        var rt = text.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(700, 40);

        return text;
    }

    private Button CriarBotao(string nome, string label, Vector2 anchoredPos, Transform parent, UnityEngine.Events.UnityAction onClick)
    {
        GameObject btnObj = new GameObject(nome);
        btnObj.transform.SetParent(parent, false);

        var image = btnObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.35f, 0.7f, 1f);

        var button = btnObj.AddComponent<Button>();
        button.onClick.AddListener(onClick);

        var rt = btnObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(130, 45);

        var txt = CriarTexto($"{nome}_Label", new Vector2(10, -8), 16, btnObj.transform);
        txt.text = label;

        return button;
    }
}
