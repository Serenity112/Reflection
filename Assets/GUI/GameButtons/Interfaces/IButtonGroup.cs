using System;
using System.Collections.Generic;

public abstract class IButtonGroup : IDraggableButton
{
    private IButtonManager _manager;

    public void SetManager(IButtonManager manager) => _manager = manager;

    public IButtonManager GetManager() => _manager;

    public abstract void RegisterManager();

    private List<Action> OnAwakeActionsList = new List<Action>();

    private List<Action> OnStartActionsList = new List<Action>();

    public void OnAwakeActions(List<Action> actions)
    {
        OnAwakeActionsList.AddRange(actions);
    }

    public void OnStartActions(List<Action> actions)
    {
        OnStartActionsList.AddRange(actions);
    }

    public void Awake()
    {
        OnAwakeActionsList.ForEach(x => x.Invoke());
    }

    public void Start()
    {
        RegisterManager();
        GetManager().SubscribeButton(gameObject);

        OnStartActionsList.ForEach(x => x.Invoke());
    }

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
