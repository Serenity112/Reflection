using UnityEngine;

public abstract class IButtonGroup : IDraggableButton
{
    private IButtonManager _manager;

    public void SetManager(IButtonManager manager) => _manager = manager;

    public IButtonManager GetManager() => _manager;

    public abstract void RegisterManager();

    public void Start()
    {
        RegisterManager();
        GetManager().SubscribeButton(gameObject);
        OnStart();
    }

    public virtual void OnStart() { }

    public void Awake()
    {
        OnAwake();
    }

    public abstract void OnAwake();

    public override void PrePointerDown()
    {
        GetManager().ButtonSelected = true;
    }

    public override void PrePointerUp()
    {
        GetManager().ButtonSelected = false;
        GetManager().AppearActualButton();
    }

    public override bool PointerEnterCondition()
    {
        return !GetManager().ButtonSelected;
    }
}
