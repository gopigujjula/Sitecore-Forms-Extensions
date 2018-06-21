﻿using Feature.FormsExtensions.Business.PrefillToken;
using Feature.FormsExtensions.Fields.Prefill;
using Sitecore.ExperienceForms.Mvc.Pipelines.RenderField;
using Sitecore.Mvc.Pipelines;

namespace Feature.FormsExtensions.Pipelines.RenderField
{
    public class ReplacePrefillToken : MvcPipelineProcessor<RenderFieldEventArgs>
    {
        private readonly IPrefillTokenMapFactory prefillTokenMapFactory;

        public ReplacePrefillToken(IPrefillTokenMapFactory prefillTokenMapFactory)
        {
            this.prefillTokenMapFactory = prefillTokenMapFactory;
        }

        public override void Process(RenderFieldEventArgs args)
        {
            if (!(args.ViewModel is IBindingSettings bindingSettings))
            {
                return;
            }
            if (string.IsNullOrEmpty(bindingSettings.BindingToken))
            {
                return;
            }

            var tokenMap = prefillTokenMapFactory.GetPrefillTokenMap();
            if (tokenMap == null || !tokenMap.ContainsKey(new PrefillTokenKey(bindingSettings.BindingToken)))
                return;

            var tokenHandler = tokenMap[new PrefillTokenKey(bindingSettings.BindingToken)];
            if (tokenHandler == null)
            {
                return;
            }

            var value = tokenHandler.GetTokenValue();
            if (!value.HasValue())
            {
                return;
            }

            var property = args.ViewModel.GetType().GetProperty("Value");
            if (value.Value.GetType() != property?.PropertyType)
            {
                return;
            }
            property.SetValue(args.ViewModel, value.Value);
        }


    }
}