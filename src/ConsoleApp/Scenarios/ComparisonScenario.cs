using System.Text.Json;
using Testar.ChangeDetection.Core;

namespace Testar.ChangeDetection.ConsoleApp.Scenarios;

public class ComparisonScenario : IScenario
{
    private readonly IChangeDetectionHttpClient client;

    public ComparisonScenario(IChangeDetectionHttpClient client)
    {
        this.client = client;
    }

    public async Task RunAsync()
    {
        await client.LoginAsync(new Uri("http://localhost:5000/"), new LoginModel { Username = "testar2\\testar", Password = "testar" });
        var modelIdentifier = "1chdi5230521708089";

        var abstractStates = await new OrientDbCommand("SELECT FROM AbstractState WHERE modelIdentifier = :modelIdentifier")
            .AddParameter("modelIdentifier", modelIdentifier)
            .ExecuteOn<JsonElement>(client)
            .Select(ToAbstractState)
            .ToListAsync();

        var abstractActions = await new OrientDbCommand("SELECT FROM AbstractAction WHERE modelIdentifier = :modelIdentifier")
            .AddParameter("modelIdentifier", modelIdentifier)
            .ExecuteOn<JsonElement>(client)
            .Select(ToAbstractAction)
            .ToListAsync();

        var concreteState = await new OrientDbCommand("SELECT FROM (TRAVERSE in() FROM (SELECT FROM AbstractState WHERE modelIdentifier = :modelIdentifier)) WHERE @class = 'ConcreteState'")
            .AddParameter("modelIdentifier", modelIdentifier)
            .ExecuteOn<JsonElement>(client)
            .Select(ToConcreteState)
            .ToListAsync();

        var concreteActions = await new OrientDbCommand("SELECT FROM (TRAVERSE in('isAbstractedBy').outE('ConcreteAction') FROM (SELECT FROM AbstractState WHERE modelIdentifier = :modelIdentifier)) WHERE @class = 'ConcreteAction'")
            .AddParameter("modelIdentifier", modelIdentifier)
            .ExecuteOn<JsonElement>(client)
            .Select(ToConcreteAction)
            .ToListAsync();

        foreach (var abstractState in abstractStates)
        {
            foreach (var concreteStateId in abstractState.ConcreteStateIds)
            {
                var state = concreteState.FirstOrDefault(x => x.ConcreteStateId == concreteStateId);
                if (state is not null)
                {
                    abstractState.AddConcreteState(state);
                }
            }
        }

        foreach (var abstractAction in abstractActions)
        {
            foreach (var abstractActionId in abstractAction.ConcreteActionIds)
            {
                var action = concreteActions.FirstOrDefault(x => x.ConcreteActionId == abstractActionId);
                if (action is not null)
                {
                    abstractAction.AddConcreteAction(action);
                }
            }
        }
    }

    public AbstractState ToAbstractState(JsonElement json)
    {
        return new AbstractState
        {
            Properties = json
            .EnumerateObject()
            .ToDictionary(x => x.Name, x => x.Value.ToString().Replace("\"", ""))
        };
    }

    public AbstractAction ToAbstractAction(JsonElement json)
    {
        return new AbstractAction
        {
            Properties = json
            .EnumerateObject()
            .ToDictionary(x => x.Name, x => x.Value.ToString().Replace("\"", ""))
        };
    }

    public ConcreteState ToConcreteState(JsonElement json)
    {
        return new ConcreteState
        {
            Properties = json
            .EnumerateObject()
            .ToDictionary(x => x.Name, x => x.Value.ToString().Replace("\"", ""))
        };
    }

    public ConcreteAction ToConcreteAction(JsonElement json)
    {
        return new ConcreteAction
        {
            Properties = json
            .EnumerateObject()
            .ToDictionary(x => x.Name, x => x.Value.ToString().Replace("\"", ""))
        };
    }

    public TestarChangedModel CalculateChanges(TestarModel model1, TestarModel model2)
    {
        return new TestarChangedModel
        {
            AddedProperties = model2.Properties
                .Where(m2 => model1.Properties.Any(m1 => m2.Key != m1.Key))
                .Select(x => new AddedProperty
                {
                    Name = x.Key,
                    Value = x.Value
                })
                .ToList(),

            RemovedProperties = model1.Properties
                .Where(m1 => model2.Properties.Any(m2 => m1.Key != m2.Key))
                .Select(x => new RemovedProperty
                {
                    Name = x.Key,
                    Value = x.Value
                })
                .ToList(),

            ChangedProperties = model1.Properties
                .Where(m2 => model2.Properties.ContainsKey(m2.Key))
                .Select(x => new ChangedProperty
                {
                    Name = x.Key,
                    OldValue = x.Value,
                    NewValue = model2.Properties[x.Key]
                })
                .ToList()
        };
    }
}

public class TestarModel
{
    public Dictionary<string, string> Properties = new Dictionary<string, string>();
    public OrientDbId Id => new OrientDbId(Properties["@rid"]);

    protected string[] ParseArray(string propertyName) => Properties[propertyName].ToString().Replace("[", "").Replace("]", "").Split(',');
}

public class ChangedProperty : ModelProperty
{
    public string OldValue { get; set; }
    public string NewValue { get; set; }

    public bool HasChange => OldValue.Equals(NewValue);
    public bool IsDifferent => !HasChange;
}

public class AddedProperty : ModelProperty
{
    public string Value { get; set; }
}

public class RemovedProperty : ModelProperty
{
    public string Value { get; set; }
}

public class ModelProperty
{
    public string Name { get; set; }
}

public class TestarChangedModel
{
    public List<AddedProperty> AddedProperties { get; set; }
    public List<RemovedProperty> RemovedProperties { get; set; }
    public List<ChangedProperty> ChangedProperties { get; set; }
}

public class AbstractState : TestarModel
{
    private List<ConcreteState> concreteStates = new();

    public ConcreteStateId[] ConcreteStateIds => ParseArray("concreteStateIds")
        .Select(x => new ConcreteStateId(x.Trim()))
        .ToArray();

    public void AddConcreteState(ConcreteState concreteState) => concreteStates.Add(concreteState);
}

public class AbstractAction : TestarModel
{
    private List<ConcreteAction> concreteActions = new();

    public ConcreteActionId[] ConcreteActionIds => ParseArray("concreteActionIds")
       .Select(x => new ConcreteActionId(x.Trim()))
       .ToArray();

    public void AddConcreteAction(ConcreteAction concreteAction) => concreteActions.Add(concreteAction);
}

public class ConcreteState : TestarModel
{
    public ConcreteStateId ConcreteStateId => new ConcreteStateId(Properties["ConcreteIDCustom"]);
}

public class ConcreteAction : TestarModel
{
    public ConcreteActionId ConcreteActionId => new ConcreteActionId(Properties["actionId"]);
}