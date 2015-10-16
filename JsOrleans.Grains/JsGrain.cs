using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsOrleans.GrainInterfaces;
using Orleans;
using EdgeJs;

namespace JsOrleans.Grains
{

    public class JsGrainState : GrainState
    {
        public string JavaScript { get; set; }
        public string Json { get; set; }
    }


    public class JsGrain : Grain<JsGrainState>, IJsGrain
    {
        public async Task Become(string javaScript)
        {
            this.State.JavaScript = javaScript;
            await this.WriteStateAsync();
        }

        public async Task<string> Execute(string message)
        {
            // if the grain hasn't 'Become', just echo
            if (string.IsNullOrWhiteSpace(this.State.JavaScript)) this.State.JavaScript = "done(null, message, state)";

            var javaScript = @"
                return function (data, _callback) {
                     var state = JSON.parse(data.state);
                     var message = JSON.parse(data.message);
                     var done = function(err, outputMessage, newState){
                        _callback(null, {newState: JSON.stringify(newState), message : JSON.stringify(outputMessage)});
                     }
                     " + this.State.JavaScript + @"
                }
            ";

            var func = Edge.Func(javaScript);
            
            dynamic result = (await func(new { state = this.State.Json ?? "{}", message = message ?? "{}"}));

            if (result.newState != this.State.Json)
            {
                // if the JavaScript returns a mutated state, save it
                this.State.Json = result.newState as string;
                await this.WriteStateAsync();
            }
            return result.message as string;
        }

    }
}
