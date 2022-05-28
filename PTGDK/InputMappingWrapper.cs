

public static class InputMappingWrapper
{
    private static readonly InputMapping instance;
    private static bool inputMappingIsEnabled;
    
    static InputMappingWrapper()
    {
        instance = new InputMapping();
        inputMappingIsEnabled = false;
    }

    public static InputMapping Get()
    {
        return instance;
    }

    public static void Enable()
    {
        if (!inputMappingIsEnabled)
        {
            instance.Enable();
            inputMappingIsEnabled = true;
        }
    }

    public static void Disable()
    {
        if (inputMappingIsEnabled)
        {
            instance.Disable();
            inputMappingIsEnabled = false;
        }
    }
}
