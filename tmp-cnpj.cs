using System;
class P {
  static int Val(char c) => c - '0';
  static string CalcDv(string base12) {
    int[] pesos = {6,5,4,3,2,9,8,7,6,5,4,3,2};
    int s1=0,s2=0;
    for(int i=0;i<12;i++){ int v=Val(base12[i]); s1+=v*pesos[i+1]; s2+=v*pesos[i]; }
    int dv1 = s1%11<2?0:11-(s1%11);
    s2 += dv1*pesos[12];
    int dv2 = s2%11<2?0:11-(s2%11);
    return $"{dv1}{dv2}";
  }
  static void Main(){
    foreach(var cnpj in new[]{"8H9XJ6M00001","12ABC34501DE","06570588000103".Substring(0,12)}){
      Console.WriteLine(cnpj+" -> "+CalcDv(cnpj));
    }
  }
}
