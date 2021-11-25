namespace Testaufgabe_Dynamics_Consulting_PlugIns
{
  public class PreAccountCreatedPlugIn : Microsoft.Xrm.Sdk.IPlugin
  {
    /// <summary>
    /// Throws InvalidPluginExecutionException if there may be a Name-Doublette
    /// </summary>
    /// <param name="serviceProvider"></param>
    public void Execute(System.IServiceProvider serviceProvider)
    {
      Microsoft.Xrm.Sdk.IPluginExecutionContext pluginExecutionContext = (Microsoft.Xrm.Sdk.IPluginExecutionContext)
        serviceProvider.GetService(typeof(Microsoft.Xrm.Sdk.IPluginExecutionContext));

      if (!pluginExecutionContext.InputParameters.Contains(Constants.Entity.TARGET) ||
          !(pluginExecutionContext.InputParameters[Constants.Entity.TARGET] is Microsoft.Xrm.Sdk.Entity))
      {
        return;
      }

      Microsoft.Xrm.Sdk.Entity entity = (Microsoft.Xrm.Sdk.Entity)pluginExecutionContext.InputParameters[Constants.Entity.TARGET];

      if (entity.LogicalName != Constants.Entity.ACCOUNT)
      {
        return;
      }

      if (!entity.Attributes.Contains(Constants.Account.NAME))
      {
        throw new Microsoft.Xrm.Sdk.InvalidPluginExecutionException("The Name of the Account is necessary!");
      }

      Microsoft.Xrm.Sdk.IOrganizationServiceFactory organizationServiceFactory = (Microsoft.Xrm.Sdk.IOrganizationServiceFactory)
        serviceProvider.GetService(typeof(Microsoft.Xrm.Sdk.IOrganizationServiceFactory));

      Microsoft.Xrm.Sdk.IOrganizationService organizationService =
        organizationServiceFactory.CreateOrganizationService(pluginExecutionContext.UserId);

      Microsoft.Xrm.Sdk.Query.QueryExpression queryExpression = new Microsoft.Xrm.Sdk.Query.QueryExpression(Constants.Entity.ACCOUNT);

      queryExpression.Criteria.AddCondition(
        Constants.Account.NAME,
        Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, entity.GetAttributeValue<string>(Constants.Account.NAME));

      Microsoft.Xrm.Sdk.EntityCollection entityCollection = organizationService.RetrieveMultiple(queryExpression);

      if (entityCollection == null || entityCollection.Entities == null || entityCollection.Entities.Count < 1)
      {
        return;
      }

      throw new Microsoft.Xrm.Sdk.InvalidPluginExecutionException($"Account with name: {entity.GetAttributeValue<string>(Constants.Account.NAME)} still exists!");
    }
  }
}