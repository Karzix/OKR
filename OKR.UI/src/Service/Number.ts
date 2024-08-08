export function isNumberOrNumericString(data: any): boolean {
    if (typeof data === 'number') {
      return true;
    }
    if (typeof data === 'string') {
      return !isNaN(data as any) && !isNaN(parseFloat(data));
    }
    return false;
  }