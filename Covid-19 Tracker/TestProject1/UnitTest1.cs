using System;
using System.Collections.Generic;
using Xunit;

namespace TestProject1
{
    public class UnitTest1
    {
        public String JsonStr = 
            @"{
            ""modified"": ""2021-05-12T08:19:46+02:00"",
            ""data"": [
                {
                ""datum"": ""2021-05-12"",
                ""provedene_testy_celkem"": 6937202,
                ""ockovane_osoby_vcerejsi_den"": 71027,
                ""ockovane_osoby_vcerejsi_den_datum"": ""2021-05-11""
                }
            ]
        }";

        [Fact]
        public void JsonParse()
        {
            ;
        }   
    }
}