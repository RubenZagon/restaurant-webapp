// Product and category translations
// Spanish names come from backend, English translations defined here

export const categoryTranslations: Record<string, { en: string; es: string }> = {
  'Bebidas': { en: 'Drinks', es: 'Bebidas' },
  'Entrantes': { en: 'Starters', es: 'Entrantes' },
  'Carnes a la Brasa': { en: 'Grilled Meats', es: 'Carnes a la Brasa' },
  'Pescados': { en: 'Fish & Seafood', es: 'Pescados' },
  'Guisos Canarios': { en: 'Canarian Stews', es: 'Guisos Canarios' },
  'Postres': { en: 'Desserts', es: 'Postres' },
  'Cafés': { en: 'Coffee', es: 'Cafés' }
}

export const categoryDescriptionTranslations: Record<string, { en: string; es: string }> = {
  'Vinos de la casa y bebidas': { en: 'House wines and drinks', es: 'Vinos de la casa y bebidas' },
  'Para abrir boca': { en: 'To whet your appetite', es: 'Para abrir boca' },
  'Nuestras especialidades a la parrilla': { en: 'Our grill specialties', es: 'Nuestras especialidades a la parrilla' },
  'Pescado fresco del día': { en: 'Fresh fish of the day', es: 'Pescado fresco del día' },
  'Potajes y guisos tradicionales': { en: 'Traditional stews and potages', es: 'Potajes y guisos tradicionales' },
  'Postres caseros': { en: 'Homemade desserts', es: 'Postres caseros' },
  'Café y bebidas calientes': { en: 'Coffee and hot drinks', es: 'Café y bebidas calientes' }
}

export const productTranslations: Record<string, { en: string; es: string }> = {
  // Bebidas
  'Vino Tinto de la Casa': { en: 'House Red Wine', es: 'Vino Tinto de la Casa' },
  'Vino Blanco de la Casa': { en: 'House White Wine', es: 'Vino Blanco de la Casa' },
  'Vino Rosado': { en: 'Rosé Wine', es: 'Vino Rosado' },
  'Agua': { en: 'Water', es: 'Agua' },
  'Refresco': { en: 'Soft Drink', es: 'Refresco' },
  'Cerveza Dorada': { en: 'Dorada Beer', es: 'Cerveza Dorada' },
  'Tropical': { en: 'Tropical Beer', es: 'Tropical' },

  // Entrantes
  'Papas Arrugadas con Mojo': { en: 'Wrinkled Potatoes with Mojo', es: 'Papas Arrugadas con Mojo' },
  'Queso Asado': { en: 'Grilled Cheese', es: 'Queso Asado' },
  'Pimientos de Padrón': { en: 'Padrón Peppers', es: 'Pimientos de Padrón' },
  'Chicharrones': { en: 'Pork Cracklings', es: 'Chicharrones' },
  'Chorizo a la Brasa': { en: 'Grilled Chorizo', es: 'Chorizo a la Brasa' },
  'Champiñones al Ajillo': { en: 'Garlic Mushrooms', es: 'Champiñones al Ajillo' },

  // Carnes
  'Chuletas de Cerdo': { en: 'Pork Chops', es: 'Chuletas de Cerdo' },
  'Costillas': { en: 'Ribs', es: 'Costillas' },
  'Pollo al Horno': { en: 'Roasted Chicken', es: 'Pollo al Horno' },
  'Conejo al Salmorejo': { en: 'Rabbit in Salmorejo', es: 'Conejo al Salmorejo' },
  'Carne de Cabra': { en: 'Goat Stew', es: 'Carne de Cabra' },
  'Entrecot': { en: 'Ribeye Steak', es: 'Entrecot' },

  // Pescados
  'Cherne a la Plancha': { en: 'Grilled Wreckfish', es: 'Cherne a la Plancha' },
  'Vieja Saneada': { en: 'Grilled Parrotfish', es: 'Vieja Saneada' },
  'Sama a la Plancha': { en: 'Grilled Sea Bream', es: 'Sama a la Plancha' },
  'Pulpo a la Gallega': { en: 'Galician Octopus', es: 'Pulpo a la Gallega' },
  'Calamares Fritos': { en: 'Fried Calamari', es: 'Calamares Fritos' },

  // Guisos
  'Ropa Vieja': { en: 'Ropa Vieja Stew', es: 'Ropa Vieja' },
  'Potaje de Berros': { en: 'Watercress Stew', es: 'Potaje de Berros' },
  'Puchero Canario': { en: 'Canarian Puchero', es: 'Puchero Canario' },
  'Rancho Canario': { en: 'Canarian Rancho', es: 'Rancho Canario' },

  // Postres
  'Quesillo': { en: 'Canarian Flan', es: 'Quesillo' },
  'Bienmesabe': { en: 'Almond Cream Dessert', es: 'Bienmesabe' },
  'Frangollo': { en: 'Gofio Pudding', es: 'Frangollo' },
  'Príncipe Alberto': { en: 'Prince Albert Cake', es: 'Príncipe Alberto' },
  'Helado de la Casa': { en: 'House Ice Cream', es: 'Helado de la Casa' },

  // Cafés
  'Café Solo': { en: 'Espresso', es: 'Café Solo' },
  'Cortado': { en: 'Cortado', es: 'Cortado' },
  'Café con Leche': { en: 'Coffee with Milk', es: 'Café con Leche' },
  'Barraquito': { en: 'Barraquito (Canarian Coffee)', es: 'Barraquito' }
}

export const productDescriptionTranslations: Record<string, { en: string; es: string }> = {
  // Bebidas
  'Vino tinto del norte de Tenerife': { en: 'Red wine from northern Tenerife', es: 'Vino tinto del norte de Tenerife' },
  'Vino blanco afrutado': { en: 'Fruity white wine', es: 'Vino blanco afrutado' },
  'Vino rosado fresco': { en: 'Fresh rosé wine', es: 'Vino rosado fresco' },
  'Agua mineral': { en: 'Mineral water', es: 'Agua mineral' },
  'Coca-Cola, Fanta, Sprite': { en: 'Coca-Cola, Fanta, Sprite', es: 'Coca-Cola, Fanta, Sprite' },
  'Cerveza canaria': { en: 'Canarian beer', es: 'Cerveza canaria' },

  // Entrantes
  'Papas con mojo picón y mojo verde': { en: 'Potatoes with spicy and green mojo', es: 'Papas con mojo picón y mojo verde' },
  'Queso de cabra asado con mojo': { en: 'Grilled goat cheese with mojo', es: 'Queso de cabra asado con mojo' },
  'Pimientos fritos con sal gorda': { en: 'Fried peppers with coarse salt', es: 'Pimientos fritos con sal gorda' },
  'Chicharrones caseros': { en: 'Homemade pork cracklings', es: 'Chicharrones caseros' },
  'Chorizo canario a la brasa': { en: 'Grilled Canarian chorizo', es: 'Chorizo canario a la brasa' },
  'Champiñones salteados con ajo': { en: 'Sautéed mushrooms with garlic', es: 'Champiñones salteados con ajo' },

  // Carnes
  'Chuletas de cerdo a la brasa con papas y ensalada': { en: 'Grilled pork chops with potatoes and salad', es: 'Chuletas de cerdo a la brasa con papas y ensalada' },
  'Costillas de cerdo a la brasa': { en: 'Grilled pork ribs', es: 'Costillas de cerdo a la brasa' },
  'Medio pollo al horno con papas': { en: 'Half roasted chicken with potatoes', es: 'Medio pollo al horno con papas' },
  'Conejo marinado en salmorejo canario': { en: 'Rabbit marinated in Canarian salmorejo', es: 'Conejo marinado en salmorejo canario' },
  'Carne de cabra guisada': { en: 'Stewed goat meat', es: 'Carne de cabra guisada' },
  'Entrecot de ternera a la brasa': { en: 'Grilled beef ribeye', es: 'Entrecot de ternera a la brasa' },

  // Pescados
  'Cherne fresco con papas arrugadas': { en: 'Fresh wreckfish with wrinkled potatoes', es: 'Cherne fresco con papas arrugadas' },
  'Vieja a la plancha': { en: 'Grilled parrotfish', es: 'Vieja a la plancha' },
  'Sama fresca con guarnición': { en: 'Fresh sea bream with garnish', es: 'Sama fresca con guarnición' },
  'Pulpo con papas y pimentón': { en: 'Octopus with potatoes and paprika', es: 'Pulpo con papas y pimentón' },
  'Calamares rebozados': { en: 'Battered calamari', es: 'Calamares rebozados' },

  // Guisos
  'Guiso de garbanzos con carne': { en: 'Chickpea stew with meat', es: 'Guiso de garbanzos con carne' },
  'Potaje canario con berros y costilla': { en: 'Canarian stew with watercress and ribs', es: 'Potaje canario con berros y costilla' },
  'Puchero con verduras y carnes': { en: 'Stew with vegetables and meats', es: 'Puchero con verduras y carnes' },
  'Rancho con fideos y papas': { en: 'Rancho with noodles and potatoes', es: 'Rancho con fideos y papas' },

  // Postres
  'Flan canario casero': { en: 'Homemade Canarian flan', es: 'Flan canario casero' },
  'Postre de almendras típico canario': { en: 'Typical Canarian almond dessert', es: 'Postre de almendras típico canario' },
  'Postre de gofio con leche': { en: 'Gofio dessert with milk', es: 'Postre de gofio con leche' },
  'Bizcocho con almendras y chocolate': { en: 'Sponge cake with almonds and chocolate', es: 'Bizcocho con almendras y chocolate' },
  'Helado artesanal': { en: 'Artisan ice cream', es: 'Helado artesanal' },

  // Cafés
  'Café expreso': { en: 'Espresso coffee', es: 'Café expreso' },
  'Café cortado': { en: 'Cortado coffee', es: 'Café cortado' },
  'Café con leche': { en: 'Coffee with milk', es: 'Café con leche' },
  'Café canario con leche condensada y licor': { en: 'Canarian coffee with condensed milk and liqueur', es: 'Café canario con leche condensada y licor' }
}

export function translateText(text: string | null, language: string): string {
  if (!text) return ''
  if (language === 'es') return text

  // Try category translation
  if (categoryTranslations[text]) {
    return categoryTranslations[text].en
  }

  // Try category description translation
  if (categoryDescriptionTranslations[text]) {
    return categoryDescriptionTranslations[text].en
  }

  // Try product translation
  if (productTranslations[text]) {
    return productTranslations[text].en
  }

  // Try product description translation
  if (productDescriptionTranslations[text]) {
    return productDescriptionTranslations[text].en
  }

  // Return original text if no translation found
  return text
}
