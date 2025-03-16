from flair.data import Sentence
from flair.nn import Classifier
import flair

class NerTag:
  def __init__(self, start_position, end_position, text, tag, score):
    self.start_position = start_position
    self.end_position = end_position
    self.text = text
    self.tag = tag
    self.score = score

def predict(text: str) -> list[NerTag]:

    # make a sentence
    sentence = Sentence(text, language_code = "fr")

    # load the NER tagger
    tagger = Classifier.load('flair/ner-french')
    
    # run NER over sentence
    tagger.predict(sentence)

    # print the sentence with all annotations
    print(sentence)

    # make example sentence
    sentence = Sentence(text)

    # predict NER tags
    tagger.predict(sentence)

    # iterate over entities and print
    # for entity in sentence.get_spans('ner'):
    #     print(entity)

    return [NerTag(entity.start_position, entity.end_position, entity.text, entity.tag, entity.score) for entity in sentence.get_spans('ner')]